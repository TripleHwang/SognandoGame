using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierShooter : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public GameObject missile;
    [SerializeField] public GameObject target;
 
    [SerializeField] public float spd;
    [SerializeField] public int shot;

    [SerializeField] public GameObject p1;

    [SerializeField] public GameObject p2;

    Vector3 initTargetPosition;

    void Start(){
        initTargetPosition =  target.transform.localPosition;
    }
 
    public void Shot() {
        StartCoroutine(CreateMissile());
    }
 
    IEnumerator CreateMissile() {
        Debug.Log("Shoot");
        int _shot = shot;
        while (_shot > 0) {
            _shot--;
            GameObject bullet = Instantiate(missile, transform);
            bullet.GetComponent<BezierObject>().master = gameObject;
            bullet.GetComponent<BezierObject>().enemy = target;
            bullet.GetComponent<BezierObject>().p1 = p1;
            bullet.GetComponent<BezierObject>().p2 = p2;
            bullet.GetComponent<BezierObject>().spd = spd;

            if(target.transform.localPosition.y < 6.2f){
                target.transform.position += new Vector3(0, 0.3f, 0);
            }
            
 
            yield return new WaitForSeconds(0.03f);
        }
        target.transform.localPosition = initTargetPosition;
        yield return null;
    }
}
