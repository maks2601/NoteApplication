using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MessageObject : NoteObject, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI authorText, messageText;

    public override void NoteInit(NoteData noteData, Action<NoteObject> noteSelected)
    {
        base.NoteInit(noteData, noteSelected);

        authorText.text = noteData.username;
        messageText.text = noteData.message;

        highlited = Color.gray;
        common = new Color(1, 1, 1, 1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        noteSelected(this);
    }

    public override void NoteHighlited()
    {
        base.NoteHighlited();
    }
}
