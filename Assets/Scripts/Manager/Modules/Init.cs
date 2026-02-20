using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour
{
    private static Init instance;

    private static ModuleManager modules;

    public List<Module> modulesInstances;

    public float timerMin = 2f;
    public float timerMax = 11f;

    private int _countPassed;

    private float _currentTimer;

    private bool _isAllInitialized;

    private  void Start()
    {
        if (instance == null)
        {
            instance = this;
            instance._countPassed = _countPassed;
        }

        if (modules == null)
        {
            modules = new ModuleManager(modulesInstances);
            ModuleManager.OnAllInitialized += OnInitializedFirst;
        }
        else
        {
            modules.AddNewModules(modulesInstances);
            ModuleManager.OnAllInitialized += OnInitializedSecond;
        }

        ModuleManager.StartInitialize();
    }

    private void Update()
    {
        _currentTimer += Time.deltaTime;

        if (_currentTimer > timerMin && _isAllInitialized || _currentTimer > timerMax)
        {
            SceneManager.LoadScene(1);
        }
    }

    private void OnInitializedFirst()
    {
        _isAllInitialized = true;

        ModuleManager.OnAllInitialized -= OnInitializedFirst;
    }

    private void OnInitializedSecond()
    {
        _isAllInitialized = true;

        ModuleManager.OnAllInitialized -= OnInitializedSecond;
    }
}