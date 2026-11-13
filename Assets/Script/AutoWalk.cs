using UnityEngine;
using System.Collections;

public class AutoWalk : MonoBehaviour
{
    public float speed = 1.5f;
    public float distance = 3f;
    public float jumpHeight = 0.5f;
    public float jumpTime = 0.5f;

    float startX;
    int dir = 1;
    bool isJumping;
    SpriteRenderer sr;
    Animator anim;

    void Start()
    {
        startX = transform.position.x;
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isJumping) return;   // stop walking while jumping

        transform.Translate(Vector3.right * dir * speed * Time.deltaTime);

        if (transform.position.x > startX + distance) dir = -1;
        else if (transform.position.x < startX - distance) dir = 1;

        sr.flipX = (dir == 1);
    }

    public void Jump()
    {
        if (!isJumping) StartCoroutine(JumpRoutine());
    }

    IEnumerator JumpRoutine()
    {
        isJumping = true;
        anim.SetTrigger("Jump");

        float startY = transform.position.y;
        float t = 0f;
        while (t < jumpTime)
        {
            t += Time.deltaTime;
            float y = Mathf.Sin(t / jumpTime * Mathf.PI) * jumpHeight;
            transform.position = new Vector3(transform.position.x, startY + y, transform.position.z);
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, startY, transform.position.z);
        isJumping = false;
    }
    public void JumpThenLoad(string sceneName)
    {
        if (!isJumping) StartCoroutine(JumpLoadRoutine(sceneName));
    }

    IEnumerator JumpLoadRoutine(string sceneName)
    {
        yield return JumpRoutine();   // wait until the jump finishes
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}