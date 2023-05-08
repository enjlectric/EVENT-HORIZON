using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level/Intro", fileName = "Intro")]
public class LevelIntro : GameSection
{
    public float cameraStartY;
    public AnimationCurve cameraIntroCurve;
    public ParticleSystem explosionPrefab;

    internal override IEnumerator ExecutionRoutine()
    {
        Manager.instance.player.transform.position = Vector3.down * 20;
        Manager.LockPlayerInput(true);
        CoroutineManager.Start(DoMoveDown());
        yield return new WaitForSeconds(duration - 1);
        //RectTransform ready = UIManager.instance.ready;
        //var readyText = UIManager.instance.readyText;
        //ready.transform.localScale = Vector3.right + Vector3.forward;
        //ready.gameObject.SetActive(true);
        //readyText.ForEach(r => r.transform.localScale = Vector3.zero);
        //ready.transform.DOScaleY(1, 0.25f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(0.2f);

        //foreach (var text in readyText)
        //{
        //    text.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBounce);
        //    yield return new WaitForSeconds(0.1f);
        //}

        yield return new WaitForSeconds(1f);

        Instantiate(explosionPrefab, Manager.instance.player.transform.position, Quaternion.identity);
        Manager.instance.player.SetVelocity(new Vector2(Random.Range(-3, 4), 16));
        Manager.LockPlayerInput(false);


        //ready.transform.DOScaleY(0.005f, 0.22f).SetEase(Ease.OutQuad);
        //foreach (var text in readyText)
        //{
        //    text.transform.DOScale(Vector3.one * 0.01f, 0.2f).SetEase(Ease.OutQuad);
        //}
        //yield return new WaitForSeconds(0.2f);
        //ready.gameObject.SetActive(false);
    }

    private IEnumerator DoMoveDown()
    {
        var cam = Camera.main;

        cam.transform.position = new Vector3(cam.transform.position.x, cameraStartY, cam.transform.position.z);
        float t = 0;
        while (t < duration)
        {
            t += Manager.deltaTime;
            Vector3 pos = cam.transform.position;
            pos.y = cameraStartY + cameraIntroCurve.Evaluate(t / duration) * -cameraStartY;
            cam.transform.position = pos;
            yield return null;
        }
    }
}
