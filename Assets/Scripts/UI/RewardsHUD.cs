using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class RewardsHUD : MonoBehaviour
{
    [SerializeField] private Transform _killRewardsParent;
    [SerializeField] private GameObject _killRewardPrefab;

    private ObjectPool<GameObject> _killRewardsPool;

    [SerializeField] private float _fadeDelay = 1f;
    [SerializeField] private float _fadeDuration = 1f;
    private Coroutine _fadeCoroutine;
    private Player _player;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _killRewardsPool = ObjectPoolingManager.Instance.GetOrCreatePool(_killRewardPrefab);
    }

    private void Start()
    {
        if (_player != null)
        {
            _player.OnKillRewarded += ShowKillRewards;
        }
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnKillRewarded -= ShowKillRewards;
        }
    }

    // Activate the rewards objects on the player hud
    private void ShowKillRewards(GameEntity killedEntity, int xp, int kp)
    {
        GameObject killRewardObj = _killRewardsPool.Get();
        killRewardObj.transform.SetParent(_killRewardsParent, false);
        ReleaseAfterTime(killRewardObj, _fadeDelay + _fadeDuration);

        KillRewardUI rewardUI = killRewardObj.GetComponent<KillRewardUI>();
        string killedEntityName = killedEntity.GetEntityData().baseData.Name;
        rewardUI.Initialize(killedEntityName, xp, kp, _fadeDelay, _fadeDuration);
    }

    // Release an object from the pool after a delay
    public void ReleaseAfterTime(GameObject obj, float delay)
    {
        StartCoroutine(ReleaseCoroutine(obj, delay));
    }

    // Apply delay and release the object at the end
    private IEnumerator ReleaseCoroutine(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        _killRewardsPool.Release(obj);
    }

}
