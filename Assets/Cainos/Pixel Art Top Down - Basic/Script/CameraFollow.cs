using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    //let camera follow target
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float lerpSpeed = 1.0f;

        public Vector3 offset;

        private Vector3 targetPos;

        private void Start()
        {
            if (target == null) return;

            offset = new Vector3(0, 1, 0);
        }

        private void Update()
        {
            if (target == null) return;

            targetPos = target.position + offset;
            if (Vector2.Distance(transform.position, targetPos) > 0.8) {
                transform.position = targetPos;
            } else {
                transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
            }
        }
    }
}
