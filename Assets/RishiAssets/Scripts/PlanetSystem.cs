using UnityEngine;
using UnityEngine.UIElements;

public class PlanetSystem : MonoBehaviour
{
    GameObject planet;
    [SerializeField] GameObject moon;
    [SerializeField] GameObject comet;
    [SerializeField] float planetRotationSpeed;
    [SerializeField] float gravity;
    [SerializeField] Vector3 cometVelocity;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        planet = gameObject;
        float distance = (comet.transform.position - transform.position).magnitude;
        cometVelocity = new Vector3(0, 0, Mathf.Sqrt(gravity/distance));
    }

    // Update is called once per frame
    void Update()
    {
        Orbit(gravity, comet);
        planet.transform.Rotate(Vector3.up * planetRotationSpeed * Time.deltaTime);

    }

    void Orbit(float gravity, GameObject obj)
    {
        Vector3 position = obj.transform.position;

        Vector3 offset = obj.transform.position - transform.position;

        float distance = Mathf.Max(offset.magnitude, 0.1f);

        Vector3 a = -gravity * offset / Mathf.Pow(distance, 3);

        cometVelocity += a * Time.deltaTime;

        position += cometVelocity * Time.deltaTime;

        obj.transform.position = position;
    }
}
