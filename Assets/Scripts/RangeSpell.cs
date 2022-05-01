using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSpell : MonoBehaviour
{
    public GameObject target;

    private void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.y);
            transform.LookAt(targetPosition);

            float distance = Vector3.Distance(target.transform.position, this.transform.position);

            if (distance > 2f)
            {
                transform.Translate(Vector3.forward * 30.0f * Time.deltaTime);
            }
            else
            {
                HitTarget();
            }
        }

    }
    void HitTarget()
    {
        Destroy(this.gameObject);
    }
}
