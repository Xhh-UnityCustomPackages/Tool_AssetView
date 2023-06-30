using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Tool.AssetView
{
    [FilePath("ProjectSettings/AssetViewToolSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public partial class AssetViewToolSettings : ScriptableSingleton<AssetViewToolSettings>
    {
        internal SerializedObject GetSerializedObject()
        {
            return new SerializedObject(instance);
        }

        void OnDisable()
        {
            Save();
        }

        public void Save()
        {
            Save(true);
        }


        [SerializeField, SerializeReference]
        private string[] m_AssetPaths = new string[] { "Assets/" };

        public string[] assetPaths => m_AssetPaths;



        public static readonly List<System.Type> _cacheFilterRuleTypesList = new List<System.Type>();
        private static readonly Dictionary<string, System.Type> _cacheFilterRuleTypes = new Dictionary<string, System.Type>();
        private static readonly Dictionary<string, IFilterRule> _cacheFilterRuleInstance = new Dictionary<string, IFilterRule>();

        public static readonly Dictionary<string, System.Type> _cacheIssueRuleTypes = new Dictionary<string, System.Type>();
        private static readonly Dictionary<string, IIssueRule> _cacheIssueRuleInstance = new Dictionary<string, IIssueRule>();

        public static readonly List<System.Type> _cacheFunctionTypesList = new List<System.Type>();
        public static readonly Dictionary<string, System.Type> _cacheFunctionTypes = new Dictionary<string, System.Type>();
        private static readonly Dictionary<string, IFunction> _cacheFunctionInstance = new Dictionary<string, IFunction>();


        public void Init()
        {
            _cacheFilterRuleTypes.Clear();
            _cacheFilterRuleInstance.Clear();
            _cacheFilterRuleTypesList.Clear();

            var customTypes = AssetViewToolsHelper.GetAssignableTypes(typeof(IFilterRule));
            for (int i = 0; i < customTypes.Count; i++)
            {
                Type type = customTypes[i];
                if (_cacheFilterRuleTypes.ContainsKey(type.Name) == false)
                {
                    _cacheFilterRuleTypesList.Add(type);
                    _cacheFilterRuleTypes.Add(type.Name, type);
                }
            }

            _cacheIssueRuleTypes.Clear();
            _cacheIssueRuleInstance.Clear();

            customTypes = AssetViewToolsHelper.GetAssignableTypes(typeof(IIssueRule));
            for (int i = 0; i < customTypes.Count; i++)
            {
                Type type = customTypes[i];
                if (_cacheIssueRuleTypes.ContainsKey(type.Name) == false)
                    _cacheIssueRuleTypes.Add(type.Name, type);
            }

            _cacheFunctionTypesList.Clear();
            _cacheFunctionTypes.Clear();
            _cacheFunctionInstance.Clear();

            customTypes = AssetViewToolsHelper.GetAssignableTypes(typeof(IFunction));
            for (int i = 0; i < customTypes.Count; i++)
            {
                Type type = customTypes[i];
                if (_cacheFunctionTypes.ContainsKey(type.Name) == false)
                {
                    _cacheFunctionTypesList.Add(type);
                    _cacheFunctionTypes.Add(type.Name, type);
                }
            }
        }

        public static IFilterRule GetFilterRuleInstance(string ruleName)
        {
            if (_cacheFilterRuleInstance.TryGetValue(ruleName, out IFilterRule instance))
                return instance;

            // 如果不存在创建类的实例
            if (_cacheFilterRuleTypes.TryGetValue(ruleName, out Type type))
            {
                instance = (IFilterRule)Activator.CreateInstance(type);
                _cacheFilterRuleInstance.Add(ruleName, instance);
                return instance;
            }
            else
            {
                throw new Exception($"{nameof(IFilterRule)}类型无效：{ruleName}");
            }
        }

        public static IIssueRule GetIssueRuleInstance(string ruleName)
        {
            if (_cacheIssueRuleInstance.TryGetValue(ruleName, out IIssueRule instance))
                return instance;

            // 如果不存在创建类的实例
            if (_cacheIssueRuleTypes.TryGetValue(ruleName, out Type type))
            {
                instance = (IIssueRule)Activator.CreateInstance(type);
                _cacheIssueRuleInstance.Add(ruleName, instance);
                return instance;
            }
            else
            {
                throw new Exception($"{nameof(IIssueRule)}类型无效：{ruleName}");
            }
        }

        public static IFunction GetFunctionInstance(string ruleName)
        {
            if (_cacheFunctionInstance.TryGetValue(ruleName, out IFunction instance))
                return instance;

            // 如果不存在创建类的实例
            if (_cacheFunctionTypes.TryGetValue(ruleName, out Type type))
            {
                instance = (IFunction)Activator.CreateInstance(type);
                _cacheFunctionInstance.Add(ruleName, instance);
                return instance;
            }
            else
            {
                throw new Exception($"{nameof(IFunction)}类型无效：{ruleName}");
            }
        }
    }
}
