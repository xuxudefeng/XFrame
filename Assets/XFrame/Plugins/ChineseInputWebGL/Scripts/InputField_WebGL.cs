﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(InputField))]
public class InputField_WebGL : MonoBehaviour
{
    [HideInInspector]
    public InputField inputField;
    [HideInInspector]
    public RectTransform RectT;
#if UNITY_WEBGL && !UNITY_EDITOR
    int selectStartIndex;//选择文本框中文字的起始位置
    int selectEndIndex;//选择文本框中文字的结束位置
    bool isFocus;
    void Start()
    {
        inputField = GetComponent<InputField>();
        InputWebGLManage.Instance.Register(gameObject.GetInstanceID(),this);
        RectT = GetComponent<RectTransform>();
        //添加unity输入框回调
        inputField.onValueChanged.AddListener(OnValueChanged);
        inputField.onEndEdit.AddListener(OnEndEdit);
        //添加获得焦点回调
        EventTrigger trigger = inputField.gameObject.GetComponent<EventTrigger>();
        if (null == trigger)
            trigger = inputField.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry e = new EventTrigger.Entry();
        e.eventID = EventTriggerType.PointerDown;
        e.callback.AddListener((data) => { OnFocus((PointerEventData)data); });
        trigger.triggers.Add(e);
    }

    void Update()
    {
        if (isFocus)
        {
            if (inputField.selectionAnchorPosition <= inputField.selectionFocusPosition)
            {
                if (selectStartIndex != inputField.selectionAnchorPosition || selectEndIndex != inputField.selectionFocusPosition)
                {
                    OnSelectRangeChanged(inputField.selectionAnchorPosition, inputField.selectionFocusPosition);
                }
            }
            else
            {
                if (selectStartIndex != inputField.selectionFocusPosition || selectEndIndex != inputField.selectionAnchorPosition)
                {
                    OnSelectRangeChanged(inputField.selectionFocusPosition, inputField.selectionAnchorPosition);
                }
            }
        }
    }

    #region ugui回调
    private void OnValueChanged(string arg0)
    {

    }
    private void OnFocus(PointerEventData pointerEventData)
    {
        MessageBox.Show("获得焦点");
        isFocus = true;
        WebGLInput.captureAllKeyboardInput = false;
        InputShowOP(inputField.text, inputField.selectionAnchorPosition + "|" + inputField.selectionFocusPosition);
    }

    public void OnSelectRangeChanged(int startIndex, int endIndex)
    {
        selectStartIndex = startIndex;
        selectEndIndex = endIndex;
        InputShowOP(inputField.text, selectStartIndex + "|" + selectEndIndex);
    }

    public void InputShowOP(string text, string indexStr)
    {
        //获取在Canvas下的位置
        Canvas canvas = transform.GetComponentInParent<Canvas>();
        float X = 0;
        float Y = 0;
        GetPosXY(canvas, transform, ref X, ref Y);

        //左边距
        float posX = Screen.width / 2 + X - RectT.rect.width * RectT.pivot.x;
        //上边距
        float posY = Screen.height / 2 - Y - RectT.rect.height * (1 - RectT.pivot.y);

        string inputRectStr = posX + "|" + posY + "|" + RectT.rect.width + "|" + RectT.rect.height;
        string fontsize = inputField.textComponent.fontSize.ToString();
        InputWebGLManage.Instance.InputShow(gameObject.GetInstanceID().ToString(), text, fontsize, indexStr, inputRectStr);
    }

    private void OnEndEdit(string str)
    {
        isFocus = false;
    }

    /// <summary>
    /// 获取在Canvas下的位置
    /// </summary>
    public void GetPosXY(Canvas canvas, Transform tran, ref float x, ref float y)
    {
        x += tran.localPosition.x;
        y += tran.localPosition.y;
        if (canvas.transform == tran.parent)
        {
            return;
        }
        else
        {
            GetPosXY(canvas, tran.parent, ref x, ref y);
        }
    }

    #endregion

    #region WebGL回调
    public void OnInputText(string text,string selectIndexStr)
    {
        inputField.text = text;

        try
        {
            int selectIndex= int.Parse(selectIndexStr);
            inputField.selectionAnchorPosition = selectIndex;
            inputField.selectionFocusPosition = selectIndex;
            selectStartIndex = selectIndex;
            selectEndIndex = selectIndex;
        }
        catch
        {
            inputField.selectionAnchorPosition = inputField.text.Length;
            inputField.selectionFocusPosition = inputField.text.Length;
        }
        inputField.Select();
        inputField.ForceLabelUpdate();
    }
    public void OnInputEnd()
    {
        WebGLInput.captureAllKeyboardInput = true;
        //inputField.DeactivateInputField();
    }

    public void SelectAll()
    {
        inputField.selectionAnchorPosition = 0;
        inputField.selectionFocusPosition = inputField.text.Length;
        inputField.Select();
        inputField.ForceLabelUpdate();
    }

    #endregion
#endif
}
