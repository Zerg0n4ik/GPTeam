using UnityEngine;
using System.Collections;
using TMPro;

public class CataclysmAnimation : MonoBehaviour
{
    public static CataclysmAnimation Instance;

    [Header("UI Settings")]
    public TextMeshProUGUI warningText;
    public float warningDuration = 2f;

    [Header("Tornado VFX Settings")]
    public ParticleSystem tornadoParticlesPrefab;
    public ParticleSystem explosionParticlesPrefab;
    public float vfxDurationPerCell = 0.5f;
    public float moveSpeed = 3f;

    [Header("Fire VFX Settings")]
    public ParticleSystem fireParticlesPrefab;
    public float fireDuration = 2f;

    [Header("Audio Settings")]
    public AudioClip tornadoLoopSound;
    public AudioClip tornadoStartSound;
    public AudioClip stormWarningSound;
    public AudioClip fireSound;
    public AudioClip explosionSound;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void ShowWarning(string message)
    {
        if (warningText != null)
        {
            warningText.text = message;
            warningText.gameObject.SetActive(true);
            Invoke(nameof(HideWarning), warningDuration);
        }
    }

    private void HideWarning()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }

    public IEnumerator PlayTornadoAnimation(int lineIndex, bool isRow, int gridSize, GameObject[,] grid)
    {
        ShowWarning("НАДВИГАЕТСЯ УРАГАН!");
        
        if (stormWarningSound != null)
        {
            audioSource.PlayOneShot(stormWarningSound);
        }

        yield return new WaitForSeconds(warningDuration);

        if (tornadoParticlesPrefab == null) yield break;

        ParticleSystem tornado = Instantiate(tornadoParticlesPrefab);
        tornado.Play();

        GameObject tornadoAudioObject = new GameObject("TornadoAudio");
        AudioSource tornadoAudio = tornadoAudioObject.AddComponent<AudioSource>();
        tornadoAudio.spatialBlend = 1f;
        tornadoAudio.rolloffMode = AudioRolloffMode.Linear;
        tornadoAudio.transform.SetParent(tornado.transform);
        tornadoAudio.transform.localPosition = Vector3.zero;

        if (tornadoStartSound != null)
        {
            AudioSource.PlayClipAtPoint(tornadoStartSound, tornado.transform.position);
        }
        
        if (tornadoLoopSound != null)
        {
            tornadoAudio.clip = tornadoLoopSound;
            tornadoAudio.loop = true;
            tornadoAudio.Play();
        }

        Vector3 previousPosition = tornado.transform.position;
        for (int i = 0; i < gridSize; i++)
        {
            int x = isRow ? lineIndex : i;
            int z = isRow ? i : lineIndex - gridSize;

            if (x >= grid.GetLength(0)) x = grid.GetLength(0) - 1;
            if (z >= grid.GetLength(1)) z = grid.GetLength(1) - 1;

            Vector3 targetPos = grid[x, z].transform.position + Vector3.up * 2f;

            float elapsed = 0;
            Vector3 startPos = tornado.transform.position;
            
            while (elapsed < vfxDurationPerCell)
            {
                tornado.transform.position = Vector3.Lerp(startPos, targetPos, elapsed/vfxDurationPerCell);
                
                Vector3 moveDir = tornado.transform.position - previousPosition;
                if (moveDir != Vector3.zero)
                {
                    tornado.transform.rotation = Quaternion.LookRotation(moveDir);
                }
                previousPosition = tornado.transform.position;
                elapsed += Time.deltaTime * moveSpeed;
                yield return null;
            }

            // Наносим урон зданию
            if (grid[x, z].transform.childCount > 0)
            {
                Factory factory = grid[x, z].GetComponentInChildren<Factory>();
                if (factory != null)
                {
                    factory.TakeDamage();
                    SpawnExplosion(grid[x, z].transform.position);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }

        tornado.Stop();
        Destroy(tornado.gameObject, 2f);
        Destroy(tornadoAudio.gameObject, 2f);
    }

    public IEnumerator PlayFireAnimation(GameObject[,] grid, int startX, int startZ)
    {
        ShowWarning("НАЧАЛСЯ ПОЖАР!");

        if (fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        ParticleSystem[] fires = new ParticleSystem[4];
        int index = 0;
        
        for (int x = startX; x < startX + 2 && x < grid.GetLength(0); x++)
        {
            for (int z = startZ; z < startZ + 2 && z < grid.GetLength(1); z++)
            {
                Vector3 pos = grid[x, z].transform.position + Vector3.up * 0.5f;
                fires[index] = Instantiate(fireParticlesPrefab, pos, Quaternion.identity);
                fires[index].Play();
                
                // Наносим урон всем зданиям в области 2x2
                if (grid[x, z].transform.childCount > 0)
                {
                    Factory factory = grid[x, z].GetComponentInChildren<Factory>();
                    if (factory != null)
                    {
                        factory.TakeDamage();
                    }
                }
                
                index++;
            }
        }

        yield return new WaitForSeconds(fireDuration);

        foreach (var fire in fires)
        {
            if (fire != null)
            {
                fire.Stop();
                Destroy(fire.gameObject, 1f);
            }
        }
    }

    private void SpawnExplosion(Vector3 position)
    {
        if (explosionParticlesPrefab == null) return;

        ParticleSystem explosion = Instantiate(explosionParticlesPrefab, position, Quaternion.identity);
        explosion.Play();

        if (explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        Destroy(explosion.gameObject, explosion.main.duration);
    }
}