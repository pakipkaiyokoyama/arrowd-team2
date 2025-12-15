using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;
using Valve.VR;
using UnityEngine.SceneManagement;

/// <summary>
/// ワープ時のプレイヤーの向きを決定する方式
/// </summary>
public enum WarpDirectionMode
{
    [Tooltip("風船（このオブジェクト）の向き（forward）を使用【推奨・デフォルト】")]
    ThisObjectForward,

    [Tooltip("ワープポイントのTransformの向き（forward）を使用")]
    WarpPointForward,

    [Tooltip("馬車からワープポイントへの方向（従来の方式）")]
    FromCarriage,

    [Tooltip("次の風船への方向")]
    ToNextBalloon,

    [Tooltip("固定方向（Fixed Directionで指定）")]
    FixedDirection
}

public class ArrowHitController : MonoBehaviour
{
    [Header("ーー リンク設定 ーー")]
    [Tooltip("次に有効化する風船（リレー用）")]
    public GameObject nextBalloon;

    [Tooltip("【NEW】ここを割ったら移動するシーン名（ゴール用）")]
    public string nextSceneName;

    [Header("自分自身（消す用）")]
    public GameObject ball;

    [Header("ワープ先の地点（StandPoint）")]
    [Tooltip("ワープ先（空欄の場合は自分のBoxColliderの中心を使用）")]
    public Transform warpPoint;
    [Tooltip("BoxColliderの中心を使用する")]
    public bool useColliderCenter = true;

    [Header("【NEW】ワープ位置の詳細調整")]
    [Tooltip("プレイヤーをどれくらい前に立たせるか")]
    public float playerOffset = 2.0f;
    [Tooltip("プレイヤーの高さオフセット（地面からの高さ調整）")]
    public float playerHeightOffset = 0f;
    [Tooltip("プレイヤーの左右オフセット")]
    public float playerSideOffset = 0f;
    [Tooltip("プレイヤーの向きを追加で回転させる角度（Y軸）")]
    public float playerRotationOffset = 0f;
    [Tooltip("ワープ時のフェード時間")]
    public float warpFadeTime = 0.3f;
    [Tooltip("ワープアニメーションを使用する")]
    public bool useWarpAnimation = false;
    [Tooltip("ワープアニメーションの速度")]
    public float warpAnimationSpeed = 5f;

    [Header("【NEW】ワープ方向の決定方式")]
    [Tooltip("プレイヤーの向き（移動方向）をどう決定するか")]
    public WarpDirectionMode directionMode = WarpDirectionMode.ThisObjectForward;
    [Tooltip("FixedDirectionモード時の固定方向")]
    public Vector3 fixedDirection = Vector3.forward;

    [Header("馬車の設定")]
    public Transform carriage;
    [Tooltip("馬車がついてくる速度")]
    public float carSpeed = 5.0f;
    [Tooltip("馬車をどれくらい後ろに配置するか")]
    public float carriageOffset = 5.0f;
    [Tooltip("馬車の向き補正")]
    public float modelCorrectionY = 0f;
    [Tooltip("馬車の高さオフセット")]
    public float carriageHeightOffset = 0f;

    [Header("ーー 【NEW】サウンド設定 ーー")]
    [Tooltip("風船に当たった時の音")]
    public AudioClip hitSound;
    [Tooltip("ヒット音の音量")]
    [Range(0f, 1f)]
    public float hitSoundVolume = 1f;

    [Tooltip("ワープ開始時の音")]
    public AudioClip warpStartSound;
    [Tooltip("ワープ開始音の音量")]
    [Range(0f, 1f)]
    public float warpStartSoundVolume = 1f;

    [Tooltip("ワープ完了時の音")]
    public AudioClip warpCompleteSound;
    [Tooltip("ワープ完了音の音量")]
    [Range(0f, 1f)]
    public float warpCompleteSoundVolume = 1f;

    [Tooltip("次の風船が現れる時の音")]
    public AudioClip balloonAppearSound;
    [Tooltip("風船出現音の音量")]
    [Range(0f, 1f)]
    public float balloonAppearSoundVolume = 1f;

    [Tooltip("AudioSourceを自動追加する")]
    public bool autoAddAudioSource = true;

    [Header("ーー 【NEW】エフェクト設定 ーー")]
    [Tooltip("風船が割れた時のエフェクト")]
    public ParticleSystem explodeFxPrefab;
    [Tooltip("割れエフェクトのスケール")]
    public float explodeFxScale = 1f;

    [Tooltip("ワープ開始時のエフェクト")]
    public ParticleSystem warpStartFxPrefab;
    [Tooltip("ワープ開始エフェクトのスケール")]
    public float warpStartFxScale = 1f;

    [Tooltip("ワープ先に表示されるエフェクト")]
    public ParticleSystem warpDestinationFxPrefab;
    [Tooltip("ワープ先エフェクトのスケール")]
    public float warpDestinationFxScale = 1f;

    [Tooltip("次の風船が現れる時のエフェクト")]
    public ParticleSystem balloonAppearFxPrefab;
    [Tooltip("風船出現エフェクトのスケール")]
    public float balloonAppearFxScale = 1f;

    [Tooltip("エフェクトの再生遅延時間")]
    public float effectDelay = 0f;

    [Header("ーー チーム開発用設定 ーー")]
    public bool isPathNode = false;
    public int pathIndex = -1;
    public float roadScaleX = 0f;

    [Header("ーー その他設定 ーー")]
    public KeyCode triggerKey = KeyCode.T;
    public string arrowTag = "Arrow";
    public string carTag = "Car";

    // 内部フラグ
    bool triggered = false;
    public bool pathActive = true;
    private AudioSource audioSource;

    void Start()
    {
        if (pathActive) SetPathActive(true);

        if (carriage == null)
        {
            GameObject carObj = GameObject.FindWithTag(carTag);
            if (carObj != null) carriage = carObj.transform;
        }

        // AudioSourceの自動追加
        if (autoAddAudioSource)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0f; // 2Dサウンド
            }
        }
    }

    public void SetPathActive(bool on)
    {
        pathActive = on;
        if (ball != null) ball.SetActive(on);
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = on;
        if (on) triggered = false;
    }

    void Update()
    {
        if (isPathNode && !pathActive) return;
        if (!triggered && Input.GetKeyDown(triggerKey)) Trigger();
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (isPathNode && !pathActive) return;

        if (other.GetComponentInParent<Arrow>() == null && !other.CompareTag(arrowTag))
            return;

        Trigger();
    }

    void Trigger()
    {
        if (triggered) return;
        triggered = true;

        Debug.Log($"[ArrowHit] {name} 命中！");

        // ヒット音を再生
        PlaySound(hitSound, hitSoundVolume);

        // 割れエフェクトを再生
        if (effectDelay > 0)
        {
            StartCoroutine(PlayEffectWithDelay(explodeFxPrefab,
                ball != null ? ball.transform.position : transform.position,
                explodeFxScale, effectDelay));
        }
        else
        {
            PlayEffect(explodeFxPrefab,
                ball != null ? ball.transform.position : transform.position,
                explodeFxScale);
        }

        WarpPlayerAndMoveCar();

        // 次の風船を有効化
        if (nextBalloon != null)
        {
            var nextScript = nextBalloon.GetComponent<ArrowHitController>();
            if (nextScript != null)
            {
                nextScript.SetPathActive(true);

                // 風船出現音とエフェクトを再生
                PlaySound(balloonAppearSound, balloonAppearSoundVolume);
                PlayEffect(balloonAppearFxPrefab, nextBalloon.transform.position, balloonAppearFxScale);
            }
        }

        StartCoroutine(HideAndDelayDestroy());

        // シーン移動
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            StartCoroutine(SafeSceneLoad(nextSceneName));
        }
    }

    IEnumerator SafeSceneLoad(string sceneName)
    {
        Debug.Log("シーン移動開始（非同期モード）...");

        SteamVR_Fade.Start(Color.black, warpFadeTime);
        yield return new WaitForSeconds(warpFadeTime);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        SteamVR_Fade.Start(Color.clear, 0.5f);
    }

    void WarpPlayerAndMoveCar()
    {
        // 【NEW】ワープ先の位置を決定
        Vector3 basePos;

        if (useColliderCenter)
        {
            // BoxColliderの中心を使用
            BoxCollider boxCol = GetComponent<BoxCollider>();
            if (boxCol != null)
            {
                basePos = boxCol.bounds.center;
            }
            else
            {
                // BoxColliderがない場合はこのオブジェクトの位置
                Debug.LogWarning($"[ArrowHit] {name} にBoxColliderがありません。オブジェクト位置を使用します。");
                basePos = transform.position;
            }
        }
        else if (warpPoint != null)
        {
            // warpPointを使用
            basePos = warpPoint.position;
        }
        else
        {
            // どちらもない場合はこのオブジェクトの位置
            basePos = transform.position;
        }

        // 【NEW】方向決定モードに応じて移動方向を計算
        Vector3 moveDir = CalculateMoveDirection(basePos);

        // プレイヤーの位置計算（詳細調整を適用）
        Vector3 forwardOffset = moveDir * playerOffset;
        Vector3 rightDir = Vector3.Cross(Vector3.up, moveDir);
        Vector3 sideOffset = rightDir * playerSideOffset;
        Vector3 heightOffset = Vector3.up * playerHeightOffset;

        Vector3 playerPos = basePos + forwardOffset + sideOffset + heightOffset;

        if (Valve.VR.InteractionSystem.Player.instance != null)
        {
            // ワープ開始音とエフェクト
            PlaySound(warpStartSound, warpStartSoundVolume);
            PlayEffect(warpStartFxPrefab,
                Valve.VR.InteractionSystem.Player.instance.transform.position,
                warpStartFxScale);

            if (useWarpAnimation)
            {
                // アニメーション付きワープ
                StartCoroutine(WarpPlayerWithAnimation(playerPos, moveDir));
            }
            else
            {
                // 即座にワープ
                Valve.VR.InteractionSystem.Player.instance.transform.position = playerPos;

                float playerY = Quaternion.LookRotation(moveDir).eulerAngles.y + playerRotationOffset;
                Valve.VR.InteractionSystem.Player.instance.transform.rotation = Quaternion.Euler(0, playerY, 0);

                // ワープ完了音とエフェクト
                PlaySound(warpCompleteSound, warpCompleteSoundVolume);
                PlayEffect(warpDestinationFxPrefab, playerPos, warpDestinationFxScale);
            }
        }

        // 【修正】馬車の移動：プレイヤーの最終位置を基準にする
        if (carriage != null)
        {
            // プレイヤーの最終位置から馬車を配置
            Vector3 carTargetPos = playerPos - (moveDir * carriageOffset);
            carTargetPos.y += carriageHeightOffset;
            StartCoroutine(MoveCarriageSmoothly(carTargetPos, moveDir));
        }
    }

    /// <summary>
    /// 【NEW】方向決定モードに応じて移動方向を計算
    /// </summary>
    Vector3 CalculateMoveDirection(Vector3 basePos)
    {
        Vector3 direction = Vector3.forward;

        switch (directionMode)
        {
            case WarpDirectionMode.ThisObjectForward:
                // 風船（このオブジェクト）のTransformの向きを使用（最も直感的）
                direction = transform.forward;
                direction.y = 0; // 水平方向のみ
                if (direction == Vector3.zero) direction = Vector3.forward;
                else direction.Normalize();
                break;

            case WarpDirectionMode.WarpPointForward:
                // ワープポイントのTransformの向きを使用
                if (warpPoint != null)
                {
                    direction = warpPoint.forward;
                    direction.y = 0; // 水平方向のみ
                    if (direction == Vector3.zero) direction = Vector3.forward;
                    else direction.Normalize();
                }
                else
                {
                    // warpPointがない場合はこのオブジェクトの向きにフォールバック
                    direction = transform.forward;
                    direction.y = 0;
                    if (direction == Vector3.zero) direction = Vector3.forward;
                    else direction.Normalize();
                }
                break;

            case WarpDirectionMode.FromCarriage:
                // 馬車からワープポイントへの方向（従来の方式）
                if (carriage != null)
                {
                    direction = (basePos - carriage.position).normalized;
                    direction.y = 0;
                    if (direction == Vector3.zero) direction = Vector3.forward;
                }
                break;

            case WarpDirectionMode.ToNextBalloon:
                // 次の風船への方向
                if (nextBalloon != null)
                {
                    direction = (nextBalloon.transform.position - basePos).normalized;
                    direction.y = 0;
                    if (direction == Vector3.zero) direction = Vector3.forward;
                }
                else
                {
                    // 次の風船がない場合はこのオブジェクトの向きにフォールバック
                    direction = transform.forward;
                    direction.y = 0;
                    if (direction == Vector3.zero) direction = Vector3.forward;
                    else direction.Normalize();
                }
                break;

            case WarpDirectionMode.FixedDirection:
                // 固定方向を使用
                direction = fixedDirection.normalized;
                direction.y = 0;
                if (direction == Vector3.zero) direction = Vector3.forward;
                else direction.Normalize();
                break;
        }

        return direction;
    }

    IEnumerator WarpPlayerWithAnimation(Vector3 targetPos, Vector3 targetDir)
    {
        Transform playerTransform = Valve.VR.InteractionSystem.Player.instance.transform;
        Vector3 startPos = playerTransform.position;
        Quaternion startRot = playerTransform.rotation;

        float targetY = Quaternion.LookRotation(targetDir).eulerAngles.y + playerRotationOffset;
        Quaternion targetRot = Quaternion.Euler(0, targetY, 0);

        float elapsed = 0f;
        float duration = 1f / warpAnimationSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // イージング（スムーズな加減速）
            t = t * t * (3f - 2f * t);

            playerTransform.position = Vector3.Lerp(startPos, targetPos, t);
            playerTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        playerTransform.position = targetPos;
        playerTransform.rotation = targetRot;

        // ワープ完了音とエフェクト
        PlaySound(warpCompleteSound, warpCompleteSoundVolume);
        PlayEffect(warpDestinationFxPrefab, targetPos, warpDestinationFxScale);
    }

    IEnumerator MoveCarriageSmoothly(Vector3 targetPos, Vector3 finalForward)
    {
        float rotationSpeed = carSpeed * 20; // 回転速度

        while (Vector3.Distance(carriage.position, targetPos) > 0.1f)
        {
            Vector3 nextPos = Vector3.MoveTowards(carriage.position, targetPos, carSpeed * Time.deltaTime);

            // 【修正】最終的な向き（finalForward）に向かって徐々に回転
            if (finalForward != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(finalForward) * Quaternion.Euler(0, modelCorrectionY, 0);
                carriage.rotation = Quaternion.RotateTowards(carriage.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            carriage.position = nextPos;
            yield return null;
        }

        // 最終位置に設定
        carriage.position = targetPos;

        // 【修正】最終的な向きも滑らかに回転させる
        if (finalForward != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(finalForward) * Quaternion.Euler(0, modelCorrectionY, 0);

            // 向きがまだ完全に合っていない場合は、少し時間をかけて回転
            float remainingAngle = Quaternion.Angle(carriage.rotation, targetRot);
            if (remainingAngle > 1f) // 1度以上ずれている場合
            {
                float rotationTime = 0.5f; // 0.5秒かけて最終的な向きに
                float elapsed = 0f;

                Quaternion startRot = carriage.rotation;

                while (elapsed < rotationTime)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / rotationTime);
                    carriage.rotation = Quaternion.Slerp(startRot, targetRot, t);
                    yield return null;
                }
            }

            // 最終的な向きを確実に設定
            carriage.rotation = targetRot;
        }
    }

    IEnumerator HideAndDelayDestroy()
    {
        if (ball != null)
        {
            foreach (var r in ball.GetComponentsInChildren<Renderer>()) r.enabled = false;
        }
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(10.0f);

        if (ball != null) ball.SetActive(false);
    }

    // ========== 【NEW】サウンド再生メソッド ==========
    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip == null) return;

        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            // AudioSourceがない場合は一時的なものを作成
            AudioSource.PlayClipAtPoint(clip, transform.position, volume);
        }
    }

    // ========== 【NEW】エフェクト再生メソッド ==========
    private void PlayEffect(ParticleSystem effectPrefab, Vector3 position, float scale)
    {
        if (effectPrefab == null) return;

        ParticleSystem fx = Instantiate(effectPrefab, position, Quaternion.identity);
        fx.transform.localScale = Vector3.one * scale;
        fx.Play();

        float duration = fx.main.duration;
        if (fx.main.loop)
        {
            duration = 5f; // ループする場合は5秒後に削除
        }

        Destroy(fx.gameObject, duration + fx.main.startLifetime.constantMax + 0.2f);
    }

    private IEnumerator PlayEffectWithDelay(ParticleSystem effectPrefab, Vector3 position, float scale, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayEffect(effectPrefab, position, scale);
    }

    // ========== 【NEW】外部から呼び出せるメソッド ==========

    /// <summary>
    /// カスタム位置にワープする
    /// </summary>
    public void WarpToPosition(Vector3 position, Vector3 forward)
    {
        if (Valve.VR.InteractionSystem.Player.instance != null)
        {
            PlaySound(warpStartSound, warpStartSoundVolume);
            PlayEffect(warpStartFxPrefab, Valve.VR.InteractionSystem.Player.instance.transform.position, warpStartFxScale);

            Valve.VR.InteractionSystem.Player.instance.transform.position = position;
            float playerY = Quaternion.LookRotation(forward).eulerAngles.y + playerRotationOffset;
            Valve.VR.InteractionSystem.Player.instance.transform.rotation = Quaternion.Euler(0, playerY, 0);

            PlaySound(warpCompleteSound, warpCompleteSoundVolume);
            PlayEffect(warpDestinationFxPrefab, position, warpDestinationFxScale);
        }
    }

    /// <summary>
    /// 手動でトリガーする（エディタデバッグ用）
    /// </summary>
    [ContextMenu("Manual Trigger")]
    public void ManualTrigger()
    {
        if (!triggered)
        {
            Trigger();
        }
    }
}