using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.UI.Tests;

namespace LayoutTests
{
    class AspectRatioFitterTests : IPrebuildSetup
    {
        const string kPrefabPath = "Assets/Resources/AspectRatioFitterTests.prefab";

        private GameObject m_PrefabRoot;
        private AspectRatioFitter m_AspectRatioFitter;
        private RectTransform m_RectTransform;

        public void Setup()
        {
#if UNITY_EDITOR
            var rootGO = new GameObject("rootGo");
            GameObject canvasGO = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas));
            canvasGO.transform.SetParent(rootGO.transform);

            var canvas = canvasGO.GetComponent<Canvas>();

            var panelGO = new GameObject("PanelObject", typeof(RectTransform));
            panelGO.transform.SetParent(canvasGO.transform);
            var panelRT = panelGO.GetComponent<RectTransform>();
            panelRT.sizeDelta = new Vector2(200, 200);

            var testGO = new GameObject("TestObject", typeof(RectTransform), typeof(AspectRatioFitter));
            testGO.transform.SetParent(panelGO.transform);
            var testRT = testGO.GetComponent<RectTransform>();
            testRT.sizeDelta = new Vector2(150, 100);

            if (!Directory.Exists("Assets/Resources/"))
                Directory.CreateDirectory("Assets/Resources/");

            PrefabUtility.SaveAsPrefabAsset(rootGO, kPrefabPath);
            GameObject.DestroyImmediate(rootGO);
#endif
        }

        [SetUp]
        public void TestSetup()
        {
            m_PrefabRoot = Object.Instantiate(Resources.Load("AspectRatioFitterTests")) as GameObject;
            m_AspectRatioFitter = m_PrefabRoot.GetComponentInChildren<AspectRatioFitter>();

            m_AspectRatioFitter.enabled = true;

            m_RectTransform = m_AspectRatioFitter.GetComponent<RectTransform>();
            m_RectTransform.sizeDelta = new Vector2(50, 50);
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(m_PrefabRoot);
            m_PrefabRoot = null;
            m_AspectRatioFitter = null;
            m_RectTransform = null;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
#if UNITY_EDITOR
            AssetDatabase.DeleteAsset(kPrefabPath);
#endif
        }

        [Test]
        public void TestEnvelopParent()
        {
            m_AspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            m_AspectRatioFitter.aspectRatio = 1.5f;

            Assert.AreEqual(300, m_RectTransform.rect.width);
        }

        [Test]
        public void TestFitInParent()
        {
            m_AspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            m_AspectRatioFitter.aspectRatio = 1.5f;

            Assert.AreEqual(200, m_RectTransform.rect.width);
        }
    }
}
