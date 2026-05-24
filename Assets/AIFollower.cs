using UnityEngine;
using Panda;
using UnityEngine.AI;

public class FollowerAI : MonoBehaviour
{
    public GameObject player;

    private NavMeshAgent agent;

    public Transform enemy;
    public Transform bulletSpawn; 
    public GameObject bulletPrefab;
    public Vector3 target;
    float rotSpeed = 5.0f;
    float visibleRange = 80.0f;
    float shotRange = 40.0f;

    // player protect
    private Drive playerDrive;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerDrive = player.GetComponent<Drive>();
    }

    // PandaBT - Tasks

    [Task]
    public bool NearPlayer()
    {
        if (player == null) return false;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        Debug.Log("Distance to player: " + distance);
        return distance <= 25f;
    }

    // ir donde esta el jugador
    [Task]
    public void SetTargetDestination()
    {
        if (player == null)
        {
            Task.current.Fail();
            return;
        }

        agent.SetDestination(player.transform.position);
        Task.current.Succeed();
    }

    // mover hacia el destino
    [Task]
    public void MoveToDestination()
    {
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void PickRandomDestinationNearPlayer()
    {
        Vector2 randomDirection2D = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(10, 20);

        Vector3 offset = new Vector3(randomDirection2D.x, 0, randomDirection2D.y) * randomDistance;
        Vector3 targetPos = player.transform.position + offset;

        Debug.Log("alrededor del jugador en la posicion: " + targetPos);

        // comprobar que esta dentro de navmesh que no sea pared
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 25f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }


    [Task]
    public bool SeeEnemy()
    {
        if (enemy == null) return false;

        // calcular aifollwer y enemigo
        Vector3 distanceVec = enemy.position - this.transform.position;

        RaycastHit hit;
        bool seeWall = false;

        Debug.DrawRay(this.transform.position, distanceVec, Color.red);

        // si el raycast golpea algo entre el enemigo y el aifollower, comprobar si es una pared
        if (Physics.Raycast(this.transform.position, distanceVec.normalized, out hit, distanceVec.magnitude))
        {
            if (hit.collider.gameObject.tag == "wall")
            {
                seeWall = true;
            }
        }

        // 3. 在 Panda 行为树面板上打印调试信息
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("wall={0}, dist={1:0.0}", seeWall, distanceVec.magnitude);

        if (distanceVec.magnitude < visibleRange && !seeWall)
        {
            Debug.Log("Veo al enemigo!");
            return true;
        }
        else
        {
            return false;
        }
    }


    [Task]
    public void TargetEnemy()
    {
        target = enemy.transform.position;
        agent.ResetPath();
        Task.current.Succeed();
    }

    [Task]
    public void LookAtTarget()
    {
        Vector3 direction = target - this.transform.position;
        direction.y = 0;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
        
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction));

        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public bool Fire()
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);
        return true;
    }

    [Task]
    public bool ShotLinedUp()
    {
        Vector3 distance = target - this.transform.position;
        if (distance.magnitude < shotRange && Vector3.Angle(this.transform.forward, distance) < 5.0f)
            return true;
        else
            return false;
    }

    [Task]
    public bool IsPlayerHealthLessThan(float health)
    {
        float currentLiveHealth = playerDrive.health;
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("CurrentHP={0}", currentLiveHealth);

        return currentLiveHealth < health;
    }

    // proteger el jugador e ir al punto medio entre los dos
    [Task]
    public void MoveToInterceptionPoint()
    {
        Vector3 middlePoint = (player.transform.position + enemy.position) / 2f;

        agent.SetDestination(middlePoint);

        if (Task.isInspected)
            Task.current.debugInfo = string.Format("DistToMid={0:0.0}", agent.remainingDistance);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            Task.current.Succeed();
        }
    }
}