using UnityEngine;

/// <summary>
/// 2D图片平铺屏幕
/// </summary>
public class SpriteTile : MonoBehaviour
{
    public SpriteRenderer m_SpriteRenderer;//2D图片渲染器
    public Camera m_RenderCamera;//相机

    private void Awake()
    {
        if (m_SpriteRenderer == null || m_RenderCamera == null || m_SpriteRenderer.transform.localScale.x == 0 || m_SpriteRenderer.transform.localScale.y == 0)
        {
            return;
        }

        float cameraOrthographicSize = m_RenderCamera.orthographicSize;
        Vector2 spriteScale = m_SpriteRenderer.transform.localScale;
        Vector2 spriteOfUnit = new Vector2(m_SpriteRenderer.sprite.rect.width / m_SpriteRenderer.sprite.pixelsPerUnit, m_SpriteRenderer.sprite.rect.height / m_SpriteRenderer.sprite.pixelsPerUnit);
        float ratioScaleY = cameraOrthographicSize * 2 / spriteOfUnit.y / spriteScale.y;
        float ratioScaleX = (Screen.width * 1f / Screen.height) * cameraOrthographicSize * 2 / spriteOfUnit.x / spriteScale.x;
        spriteScale.x *= ratioScaleX;
        spriteScale.y *= ratioScaleY;
        m_SpriteRenderer.transform.localScale = spriteScale;
    }
}
