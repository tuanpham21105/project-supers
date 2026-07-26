using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Decal
{
    public int shapeIndex = 0;
    public Color color = Color.white;
    public float x = 0.5f;
    public float y = 0.5f;
    public float scale = 0.5f;
    public int rotate = 0;

    public Decal Clone()
    {
        return new Decal
        {
            shapeIndex = shapeIndex,
            color = color,
            x = x,
            y = y,
            scale = scale,
            rotate = rotate
        };
    }
}

[Serializable]
public class Emblem
{
    public List<Decal> decals = new List<Decal>();

    public Emblem Clone()
    {
        Emblem clone = new Emblem();
        clone.decals = new List<Decal>();

        for (int i = 0; i < decals.Count; i++)
        {
            clone.decals.Add(decals[i].Clone());
        }

        return clone;
    }

    public EmblemValueObject ToValueObject()
    {
        var vo = new EmblemValueObject();
        vo.decals = new List<DecalValueObject>();

        for (int i = 0; i < decals.Count; i++)
        {
            var d = decals[i];
            vo.decals.Add(new DecalValueObject
            {
                shapeIndex = d.shapeIndex,
                color = ColorUtility.ToHtmlStringRGBA(d.color),
                xPos = d.x,
                yPos = d.y,
                scale = d.scale,
                rotate = d.rotate
            });
        }

        return vo;
    }

    public static Emblem FromValueObject(EmblemValueObject vo)
    {
        if (vo == null || vo.decals == null)
            return new Emblem();

        var emblem = new Emblem();
        emblem.decals = new List<Decal>();

        for (int i = 0; i < vo.decals.Count; i++)
        {
            var d = vo.decals[i];
            Color color = Color.white;
            if (!string.IsNullOrEmpty(d.color))
                ColorUtility.TryParseHtmlString("#" + d.color, out color);

            emblem.decals.Add(new Decal
            {
                shapeIndex = d.shapeIndex,
                color = color,
                x = d.xPos,
                y = d.yPos,
                scale = d.scale,
                rotate = d.rotate
            });
        }

        return emblem;
    }
}