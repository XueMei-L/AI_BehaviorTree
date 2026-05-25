# Práctica 03: AI - Behavior Tree (PandaBT)

**Alumna:** XueMei Lin  
**Asignatura:** Inteligencia Artificial para Videojuegos  

---

## 🎯 Objetivo (Objetivo)
El objetivo de esta práctica es modificar el proyecto base *RobotGuard* para diseñar e implementar un NPC Acompañante (Follower AI) utilizando **PandaBT** y la navegación de Unity (**NavMesh**). Este NPC debe ser capaz de escoltar al jugador de forma inteligente, tomar decisiones tácticas autónomas como atacar a los enemigos en su rango de visión, y actuar interceptando la línea de tiro cuando la salud del jugador esté en un estado crítico.

---

## 🌳 Estructura del Árbol de Comportamiento (Behavior Tree)

El árbol está diseñado jerárquicamente priorizando la seguridad física del jugador antes que las acciones ofensivas o recreativas:

```text
tree("Root")
	parallel
		repeat mute tree("Patrol")

tree("Patrol")
	sequence
		# 1. Prioridad Absoluta: Mantenerse cerca del jugador si se aleja
		fallback
			NearPlayer
			sequence
				SetTargetDestination
				MoveToDestination
		# 2. Toma de decisiones basada en el estado de salud y amenazas
		fallback
			tree("Protect")    # Si el jugador está herido y hay enemigo -> Interceptar
			tree("Attack")     # Si el jugador está a salvo pero hay enemigo -> Atacar
			tree("Wander")     # Si todo está en paz -> Patrullar alrededor