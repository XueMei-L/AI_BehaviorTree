# Práctica 03: AI - Behavior Tree (PandaBT)

**Alumna:** XueMei Lin  
**Asignatura:** Inteligencia Artificial para Videojuegos  

---

## 🎯 Objetivo (Objetivo)
El objetivo de esta práctica es modificar el proyecto base *RobotGuard* para diseñar e implementar un NPC Acompañante (Follower AI) utilizando **PandaBT** y la navegación de Unity (**NavMesh**). Este NPC debe ser capaz de escoltar al jugador de forma inteligente, tomar decisiones tácticas autónomas como atacar a los enemigos en su rango de visión, y actuar interceptando la línea de tiro cuando la salud del jugador esté en un estado crítico.

---

## 🌳 Estructura del Árbol de Comportamiento (Behavior Tree)

El árbol es lo siguiente: 

```text
tree("Root")
	parallel
		repeat mute tree("Patrol")

tree("Patrol")
	sequence
		fallback
			NearPlayer
			sequence
				SetTargetDestination
				MoveToDestination
		fallback
			tree("Protect")
			tree("Attack")
			tree("Wander")
tree("Protect")
	sequence
		while IsPlayerHealthLessThan(30.0)
			while SeeEnemy()
				MoveToInterceptionPoint
tree("Attack")
	sequence
		SeeEnemy
		TargetEnemy
		LookAtTarget
		fallback
			while ShotLinedUp
				sequence
					Wait(0.3)
					Fire
tree("Wander")
    sequence
        PickRandomDestinationNearPlayer
        MoveToDestination
        Wait(2.0)
    