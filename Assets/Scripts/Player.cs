using System;
using UnityEngine;
using UnityEngine.AI;
using System.IO.Ports;

public class Player : MonoBehaviour
{
    SerialPort stream = new SerialPort("/dev/cu.usbmodem142101", 115200);

    Cursor cursor;
    NavMeshAgent navMeshAgent;
    Shot shot;
    public Transform gunBarrel;
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        stream.Open();
        Debug.Log("Ports: " + String.Join(", ", SerialPort.GetPortNames()));
        Debug.Log("Ports: " + stream.IsOpen);

        cursor = FindObjectOfType<Cursor>();
        shot = FindObjectOfType<Shot>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
    }


    // Update is called once per frame
    void Update()
    {
        String input = stream.ReadExisting().Trim();
        bool fire = input.Contains("1000");


        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftArrow))
            dir.z = -1.0f;
        if (Input.GetKey(KeyCode.RightArrow))
            dir.z = 1.0f;
        if (Input.GetKey(KeyCode.UpArrow))
            dir.x = -1.0f;
        if (Input.GetKey(KeyCode.DownArrow))
            dir.x = 1.0f;
        navMeshAgent.velocity = dir.normalized * moveSpeed;

        Vector3 forward = cursor.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(forward.x, 0, forward.z));

        if (fire || Input.GetMouseButtonDown(0))
        {
            Debug.Log("Punch: " + input);


            // Debug.Log("Fire: "+punchByte.ToString());

            var from = gunBarrel.position;
            var target = cursor.transform.position;
            var to = new Vector3(target.x, from.y, target.z);

            var direction = (to - from).normalized;

            RaycastHit hit;
            if (Physics.Raycast(from, to - from, out hit, 100))
            {
                to = new Vector3(hit.point.x, from.y, hit.point.z);
                if (hit.transform != null)
                {
                    var zombie = hit.transform.GetComponent<Zombie>();
                    if (zombie != null)
                    {
                        zombie.Kill();
                    }
                }
            }
            else
                to = from + direction * 100;

            shot.Show(from, to);
        }
    }
}