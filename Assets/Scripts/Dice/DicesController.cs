using Cysharp.Threading.Tasks;
using Dice.Configs;
using Dice.Services;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static Dice.Services.DamageService;

public class DicesController : MonoBehaviour
{

    public DiceData diceData;

    public Transform numericalDiceTransofrm;
    public List<DiceController> numericalDices;

    private DiceController _currentDice;

    private DamageService _damage;
    private FaceBaseConfig _baseCfg;

    private DiceType _currentDiceType;

    private CalcRequest _calcRequest;
    private DiceProgress _diceProgress;
    private DiceType _attackDiceType;

    private int _previousHitFace;

    private float _boostBonusValue;

    [Inject]
    private void Construct(DamageService damageService, FaceBaseConfig baseCfg)
    {
        _damage = damageService;
        _baseCfg = baseCfg;
    }

    public void Init()
    {
        _calcRequest = new CalcRequest();

        SpawnNumericalDice();
    }

    private void SpawnNumericalDice()
    {
        _currentDiceType = Data.instance.localData.currentNumericalDice;
        DiceController createdDice = numericalDices.Find(type => type.diceType == _currentDiceType);
        _currentDice = Instantiate(createdDice, Vector3.zero, Quaternion.identity, numericalDiceTransofrm);
        _currentDice.transform.localPosition = Vector3.zero;
        _currentDice.transform.localRotation = Quaternion.Euler(0, 180, 0);
        _currentDice.SetupInfo(diceData);

        _boostBonusValue = _baseCfg.GetBoostBonus(_currentDiceType);
        _calcRequest.diceType = _currentDice.diceType;
    }

    public CalcResult GetNumericalDamageAttack(int faceIndex)
    {
        if (_attackDiceType != _currentDice.diceType)
        {
            _diceProgress = Data.instance.localData.GetOrCreateProgress(_currentDice.diceType);
            _attackDiceType = _currentDice.diceType;
        }
        DiceFaceRarity faceLevel = _diceProgress.GetFaceProgress(faceIndex).diceFaceLevel;

        _calcRequest.faceIndex = faceIndex;
        _calcRequest.faceLevel = faceLevel;
        _calcRequest.rollCrit = true;
        _calcRequest.elementalBonus = 0;

        if (IsSameHitFace(faceIndex))
        {
            _calcRequest.externalStreakBonus += _boostBonusValue;
        }
        else
        {
            _calcRequest.externalStreakBonus = 0;
        }

        return _damage.Evaluate(_calcRequest);
    }

    private bool IsSameHitFace(int faceIndex)
    {
        bool isSame = _previousHitFace == faceIndex;
        _previousHitFace = faceIndex;

        return isSame;
    }

    public async UniTask<int> RollNumerical(float attackSpeed)
    {
        return await _currentDice.Roll(attackSpeed);
    }

    public DiceType GetCurrentDiceType() => _currentDiceType;

}