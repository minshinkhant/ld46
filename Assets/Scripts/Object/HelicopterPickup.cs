﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterPickup : MonoBehaviour {
    const float RESCUE_FLY_DURATION = 2f;
    const float RESCUE_PICKUP_DURATION = 0.25f;

    [SerializeField]
    float semiMajorAxis = 200f;
    [SerializeField]
    float semiMinorAxis = 100f;
    [SerializeField]
    float speed = 0.2f;
    [SerializeField]
    Transform helicopter;
    [SerializeField]
    Transform helicopterBlades;

    float angle = 0f;

    enum State {
        Patrolling,
        Rescuing
    }
    State state;

    void Start() {
        state = State.Patrolling;
    }

    void Update() {
        switch(state) {
            case State.Patrolling:
                angle += speed * Time.deltaTime;
                if (angle >= 2 * Mathf.PI) angle -= 2 * Mathf.PI;
                transform.position = new Vector3(semiMajorAxis * Mathf.Cos(angle), semiMinorAxis * Mathf.Sin(angle));
                break;
            case State.Rescuing:
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player) {
            RescuePeople(player);
        }
    }

    void OnTriggerStay2D(Collider2D collider) {
        Player player = collider.gameObject.GetComponent<Player>();
        if (player) {
            RescuePeople(player);
        }
    }

    void RescuePeople(Player player) {
        if (player.GetPeople() > 0 && state == State.Patrolling) {
            state = State.Rescuing;
            StartCoroutine(RescuePeopleRoutine());
            player.RescuePeople();
        }
    }

    IEnumerator RescuePeopleRoutine() {
        float elapsed = 0f;
        Vector3 originalPos = helicopter.position;
        Vector3 originalBladesPos = helicopterBlades.position;

        while (elapsed <= RESCUE_FLY_DURATION) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / RESCUE_FLY_DURATION);
            helicopter.position = originalPos + Vector3.down * Easing.EaseInOutElastic(t) * 45f;
            helicopterBlades.position = originalPos + Vector3.down * Easing.EaseInOutElastic(t) * 45f;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed <= RESCUE_PICKUP_DURATION) {
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed <= RESCUE_FLY_DURATION) {
            elapsed += Time.deltaTime;
            float t = 1 - Mathf.Clamp01(elapsed / RESCUE_FLY_DURATION);
            helicopter.position = originalPos + Vector3.down * Easing.EaseInOutElastic(t) * 45f;
            helicopterBlades.position = originalPos + Vector3.down * Easing.EaseInOutElastic(t) * 45f;
            yield return null;
        }

        helicopter.position = originalPos;
        helicopterBlades.position = originalBladesPos;
        state = State.Patrolling;
    }
}