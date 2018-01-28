﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviour
{
    public enum State
    {
        Idle,
        Marching,
        Attacking
    }

    private const string bulletSpawnerName = "bulletSpawner";

    public GameObject bulletPrefab;

    /// <summary>
    /// Counted in seconds
    /// </summary>
    public float attackWaitTime = 0.5f;


    State currentState = State.Idle;

    Unit parentUnit = null;

    [HideInInspector]
    public Transform bulletSpawnerTransform;

    public Animator anim;

    CharacterController soldierCharacterController;
    NavMeshAgent navMeshAgent;

    /// <summary>
    /// Counted in seconds
    /// </summary>
    float lastAttackTime = 0;


    void Awake()
    {
        soldierCharacterController = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        bulletSpawnerTransform = gameObject.transform.Find(bulletSpawnerName);
    }

	// Use this for initialization
	void Start () {
        setState(State.Idle);

        anim = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	    switch (currentState)
	    {
	        case State.Idle:
                getLost();
	            break;

            case State.Marching:
	            if (parentUnit.lastSeenPlayerLocation != null)
                    moveTowards(parentUnit.lastSeenPlayerLocation.Value);
	            break;

            case State.Attacking:
                if (parentUnit.lastSeenPlayerLocation != null)
                    attackTowards(parentUnit.lastSeenPlayerLocation.Value);
                break;
	    }

        
	}

    void LateUpdate()
    {
        if (navMeshAgent.velocity != Vector3.zero)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    public void setState(State state)
    {
        currentState = state;
    }

    public void setUnit(Unit unit)
    {
        parentUnit = unit;
    }

    void comeToAStop()
    {
      //  anim.SetBool("isWalking", false);
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;
    }


    void moveTowards(Vector3 targetPos)
    {
      //  anim.SetBool("isWalking", true);
        NavMeshPath navMeshPath = new NavMeshPath();
        navMeshAgent.CalculatePath(targetPos, navMeshPath);
        navMeshAgent.SetPath(navMeshPath);
    }

    void getLost()
    {
        comeToAStop();
    }

    void attackTowards(Vector3 targetPos)
    {
        comeToAStop();

        targetPos.y = transform.position.y;
        transform.LookAt(targetPos);

//        Debug.Log("Now: " + Time.time);
//        Debug.Log("Last: " + lastAttackTime);

        if (Time.time - lastAttackTime > attackWaitTime)
        {
            shoot(targetPos);
        }
    }

    void shoot(Vector3 targetPos)
    {
        anim.SetTrigger("Shoot");
        GameObject bulletGameObject = Instantiate(bulletPrefab, bulletSpawnerTransform.position, bulletSpawnerTransform.rotation);
        SoldierBullet soldierBullet = bulletGameObject.GetComponent<SoldierBullet>();

        soldierBullet.setTarget(targetPos);
        lastAttackTime = Time.time;
    }
}
