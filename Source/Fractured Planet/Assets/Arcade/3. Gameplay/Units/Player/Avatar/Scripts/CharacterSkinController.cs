using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSkinController : MonoBehaviour
{
    private Animator _animator;
    private Renderer[] _characterMaterials;

    public Texture2D[] albedoList;
    [ColorUsage(true,true)]
    public Color[] eyeColors;
    public enum EyePosition { Normal, Happy, Angry, Dead}
    public EyePosition eyeState;

    private DefaultInputActions _inputActions;

    public void Awake()
    {
        _inputActions = new DefaultInputActions();
        _inputActions.Player.Enable();
    }

    // Start is called before the first frame update
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _characterMaterials = GetComponentsInChildren<Renderer>();

    }

    // Update is called once per frame
    private void Update()
    {
        if (_inputActions.Player.Fire.triggered)
        {
            switch (eyeState)
            {
                case EyePosition.Normal:
                    //ChangeMaterialSettings(1);
                    ChangeEyeOffset(EyePosition.Angry);
                    ChangeAnimatorIdle("angry");
                    break;
                case EyePosition.Angry:
                    //ChangeMaterialSettings(2);
                    ChangeEyeOffset(EyePosition.Happy);
                    ChangeAnimatorIdle("happy");
                    break;
                case EyePosition.Happy:
                    //ChangeMaterialSettings(3);
                    ChangeEyeOffset(EyePosition.Dead);
                    ChangeAnimatorIdle("dead");
                    break;
                case EyePosition.Dead:
                    //ChangeMaterialSettings(0);
                    ChangeEyeOffset(EyePosition.Normal);
                    ChangeAnimatorIdle("normal");
                    break;

            }
        }
    }

    private void ChangeAnimatorIdle(string trigger)
    {
        _animator.SetTrigger(trigger);
    }

    private void ChangeMaterialSettings(int index)
    {
        foreach (var characterMaterial in _characterMaterials)
        {
            if (characterMaterial.transform.CompareTag("PlayerEyes"))
                characterMaterial.material.SetColor("_EmissionColor", eyeColors[index]);
            else
                characterMaterial.material.SetTexture("_MainTex",albedoList[index]);
        }
    }

    private void ChangeEyeOffset(EyePosition pos)
    {
        var offset = Vector2.zero;

        switch (pos)
        {
            case EyePosition.Normal:
                offset = new Vector2(0, 0);
                break;
            case EyePosition.Happy:
                offset = new Vector2(.33f, 0);
                break;
            case EyePosition.Angry:
                offset = new Vector2(.66f, 0);
                break;
            case EyePosition.Dead:
                offset = new Vector2(.33f, .66f);
                break;
            default:
                break;
        }

        for (var i = 0; i < _characterMaterials.Length; i++)
        {
            if (_characterMaterials[i].transform.CompareTag("PlayerEyes"))
                _characterMaterials[i].material.SetTextureOffset("_MainTex", offset);
        }

        eyeState = pos;
    }
}
