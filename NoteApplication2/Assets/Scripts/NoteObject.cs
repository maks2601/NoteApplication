using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public abstract class NoteObject : MonoBehaviour
{
    public NoteData noteData;

    public Action<NoteObject> noteSelected;

    protected bool isHighlited = false;

    protected Color highlited, common;

    public virtual void NoteInit(NoteData noteData, Action<NoteObject> noteSelected)
    {
        this.noteSelected = noteSelected;
        this.noteData = noteData;
    }

    public virtual void NoteHighlited()
    {
        if (isHighlited)
        {
            GetComponent<Image>().color = common;
        }
        else
        {
            GetComponent<Image>().color = highlited;
        }

        isHighlited = !isHighlited;
    }
}
