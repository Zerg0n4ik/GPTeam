using UnityEngine;
using TMPro;
using System.Collections;
public class FireAnimation : MonoBehaviour
{
    public ParticleSystem fireParticlesPrefab;
    public float fireDuration = 2f;
    public AudioClip fireSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public IEnumerator PlayFireAnimation(GameObject[,] grid, int startX, int startZ)
    {
        // Показ предупреждения
        if (CataclysmAnimation.Instance != null)
        {
            CataclysmAnimation.Instance.ShowWarning("НАЧАЛСЯ ПОЖАР!");
        }

        // Создаем эффекты пожара для области 2x2
        ParticleSystem[] fires = new ParticleSystem[4];
        int index = 0;
        
        for (int x = startX; x < startX + 2 && x < grid.GetLength(0); x++)
        {
            for (int z = startZ; z < startZ + 2 && z < grid.GetLength(1); z++)
            {
                Vector3 pos = grid[x, z].transform.position + Vector3.up * 0.5f;
                fires[index] = Instantiate(fireParticlesPrefab, pos, Quaternion.identity);
                fires[index].Play();
                
                // Наносим урон зданиям
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

        // Проигрываем звук пожара
        if (fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        yield return new WaitForSeconds(fireDuration);

        // Уничтожаем эффекты
        foreach (var fire in fires)
        {
            if (fire != null)
            {
                fire.Stop();
                Destroy(fire.gameObject, 1f);
            }
        }
    }
}