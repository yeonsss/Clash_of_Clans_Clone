using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarController : BaseUI
{
    private enum Sliders
    {
        Bar,
    }

    private Transform _targetTransform;

    public override void Init()
    {
        Bind<Slider>(typeof(Sliders));
        Get<Slider>((int)Sliders.Bar).value = 1;
    }

    private void Update()
    {
        var pos = _targetTransform.position;
        transform.position = new Vector3(pos.x, 2, pos.z);
    }

    public void SetHpTarget(Transform targetTransform)
    {
        _targetTransform = targetTransform;
    }

    public void SetHpValue(float maxHp, float currentHp)
    {
        var bar = Get<Slider>((int)Sliders.Bar);
        var offset = Mathf.Clamp(currentHp / maxHp, 0, 1) ;
        
        if (offset == 0) gameObject.SetActive(false);
        else bar.value = offset;
    }
}
