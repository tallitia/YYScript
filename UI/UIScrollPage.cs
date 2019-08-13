using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class UIScrollPage : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    private ScrollRect target;
    private Vector3 position;

    private Vector2 startPosition;
    private float viewWidth;
    private int m_FixedPage = 0;

    public int currentPage = 1;
    public float nextPageThreshold = 100f;
    public float springStrength = 0.3f;
    public float offset = 0f;
    public bool isAbsolute = false;

    public System.Action onBegin;
    public System.Action onEnd;
    public System.Action<int> onFinish;
    private void Start()
    {
        target = GetComponent<ScrollRect>();
        viewWidth = GetComponent<RectTransform>().sizeDelta.x;

        if (target && target.content)
            startPosition = target.content.anchoredPosition;
    }

    public void SetFixedPage(int pPage)
    {
        m_FixedPage = pPage;
    }

    // 最大页数
    private int GetMaxPage()
    {
        if (m_FixedPage > 0)
            return m_FixedPage;
        return Mathf.CeilToInt(target.content.sizeDelta.x / viewWidth);
    }

    //下一页
    public void PageDown()
    {
        if (currentPage >= GetMaxPage())
            return;

        currentPage++;

        if (isAbsolute)
            position.x = -(viewWidth + offset) * (currentPage-1);
        else
            position.x = position.x - viewWidth - offset;

        target.content.DOAnchorPos(position, springStrength);
        if (onFinish != null) onFinish(currentPage);
    }

    //上一页
    public void PageUp()
    {
        if (currentPage <= 1)
            return;

        currentPage--;

        if (isAbsolute)
            position.x = -(viewWidth + offset) * (currentPage-1);
        else
            position.x = position.x + viewWidth + offset;
        
        target.content.DOAnchorPos(position, springStrength);
        if (onFinish != null) onFinish(currentPage);
    }

    //回滚
    public void PageBack()
    {
        target.content.DOAnchorPos(position, springStrength);
    }

    public void MoveToPage(int page)
    {
        if (target == null || target.content == null)
            return;

        page = Mathf.Max(1, Mathf.Min(page, GetMaxPage()));

        if (currentPage == page)
            return;

        position = target.content.anchoredPosition;
        position.x = -(viewWidth + offset) * (page-1);
        //target.content.DOAnchorPos(position, springStrength);
        target.content.anchoredPosition = position;
        currentPage = page;
        if (onFinish != null) onFinish(currentPage);
    }

    public void OnBeginDrag(PointerEventData eventdata)
    {
        if (target == null || target.content == null)
            return;

        position = target.content.anchoredPosition;
        if (onBegin != null) onBegin();
    }

    public void OnEndDrag(PointerEventData eventdata)
    {
        if (target == null || target.content == null)
            return;

        Vector3 offset = eventdata.position - eventdata.pressPosition;
        if (Mathf.Abs(offset.x) > nextPageThreshold)
        {
            if (offset.x > 0f) //上一页
                PageUp();
            else if (offset.x < 0f) //下一页
                PageDown();
        }
        else
        {
            PageBack();
        }
        if (onEnd != null) onEnd();
    }
}