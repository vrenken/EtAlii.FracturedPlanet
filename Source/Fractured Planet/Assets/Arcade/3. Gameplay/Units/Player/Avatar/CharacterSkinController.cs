using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSkinController : MonoBehaviour
{
    private Animator _animator;
    private Renderer[] _characterMaterials;

    public Texture2D[] albedoList;

    [ColorUsage(true,true)]
    public Color[] eyeColors;

    public EyePosition eyeState;

    private DefaultInputActions _inputActions;
    private static readonly int _emissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int _mainTex = Shader.PropertyToID("_MainTex");

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

    public void ChangeMaterialSettings(int playerNumber)
    {
        var index = playerNumber - 1;
        foreach (var characterMaterial in _characterMaterials)
        {
            if (characterMaterial.transform.CompareTag("PlayerEyes"))
            {
                characterMaterial.material.SetColor(_emissionColor, eyeColors[index]);
            }
            else
            {
                characterMaterial.material.SetTexture(_mainTex,albedoList[index]);
            }
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

        foreach (var characterMaterial in _characterMaterials)
        {
            if (characterMaterial.transform.CompareTag("PlayerEyes"))
            {
                characterMaterial.material.SetTextureOffset(_mainTex, offset);
            }
        }

        eyeState = pos;
    }
}
