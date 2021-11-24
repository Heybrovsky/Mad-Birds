using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public LineRenderer[] lineRenderers;
    public Transform[] stripPositions;
    public Transform center;
    public Transform idlePosition;
    public Vector3 currentPosition;
    public float maxLength;
    public float bottomBoundary;
    public float playerPositionOffset;
    public float force;
    public GameObject playerPrefab;
    Rigidbody2D player;
    Collider2D playerCollider;

    bool isMouseDown;
    void Start()
    {
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        CreatePlayer();

    }

    void CreatePlayer()
    {
        player = Instantiate(playerPrefab).GetComponent<Rigidbody2D>();
        playerCollider = player.GetComponent<Collider2D>();
        playerCollider.enabled = false;

        player.isKinematic = true;
        ResetStrips();
    }

    void Update()
    {
        if (isMouseDown == true)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;

            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            currentPosition = center.position + Vector3.ClampMagnitude(currentPosition - center.position, maxLength);
            currentPosition = ClampBoundary(currentPosition);
            SetStrips(currentPosition);

            if (playerCollider)
            {
                playerCollider.enabled = true;
            }
        } else if (isMouseDown == false)
        {
            ResetStrips();
        }
    }

    private void OnMouseDown()
    {
        isMouseDown = true;
    }

    private void OnMouseUp()
    {
        isMouseDown = false;
        Shoot();
    }
    void ResetStrips()
    {
        currentPosition = idlePosition.position;
        SetStrips(currentPosition);
    }
    void SetStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1, position);
        lineRenderers[1].SetPosition(1, position);

        if (player)
        {
            Vector3 dir = position - center.position;
            player.transform.position = position + dir.normalized * playerPositionOffset;
            player.transform.right = -dir.normalized;
        }
    }
    void Shoot()
    {
        player.isKinematic = false;
        Vector3 playerForce = (currentPosition - center.position) * force * -1;
        player.velocity = playerForce;
        player = null;
        playerCollider = null;
        Invoke("CreatePlayer",1);
    }
    Vector3 ClampBoundary(Vector3 vector)
    {
        vector.y = Mathf.Clamp(vector.y, bottomBoundary, 1000);
        return vector;
    }
}
