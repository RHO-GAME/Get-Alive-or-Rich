using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

namespace UnityEditor.U2D.Sprites
{
    internal class SpriteRectModel : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// List of all SpriteRects
        /// </summary>
        [SerializeField] private List<SpriteRect> m_SpriteRects;
        /// <summary>
        /// List of all names in the Name-FileId Table
        /// </summary>
        [SerializeField] private List<string> m_SpriteNames;
        /// <summary>
        /// List of all FileIds in the Name-FileId Table
        /// </summary>
        [SerializeField] private List<long> m_SpriteFileIds;
        /// <summary>
        /// HashSet of all names currently in use by SpriteRects
        /// </summary>
        private HashSet<string> m_NamesInUse;

        public IReadOnlyList<SpriteRect> spriteRects => m_SpriteRects;
        public IReadOnlyList<string> spriteNames => m_SpriteNames;
        public IReadOnlyList<long> spriteFileIds => m_SpriteFileIds;

        private SpriteRectModel()
        {
            m_SpriteNames = new List<string>();
            m_SpriteFileIds = new List<long>();
        }

        public void SetSpriteRects(List<SpriteRect> newSpriteRects)
        {
            m_SpriteRects = newSpriteRects;

            m_NamesInUse = new HashSet<string>();
            for (var i = 0; i < m_SpriteRects.Count; ++i)
                m_NamesInUse.Add(m_SpriteRects[i].name);
        }

        public void SetNameFileIdPairs(IEnumerable<SpriteNameFileIdPair> pairs)
        {
            m_SpriteNames.Clear();
            m_SpriteFileIds.Clear();

            foreach (var pair in pairs)
                AddNameFileIdPair(pair.name, pair.fileId);
        }

        public int FindIndex(Predicate<SpriteRect> match)
        {
            int i = 0;
            foreach (var spriteRect in m_SpriteRects)
            {
                if (match.Invoke(spriteRect))
                    return i;
                i++;
            }
            return -1;
        }

        public void Clear()
        {
            m_SpriteRects = new List<SpriteRect>();
            m_NamesInUse = new HashSet<string>();

            m_SpriteNames.Clear();
            m_SpriteFileIds.Clear();
        }

        public bool Add(SpriteRect spriteRect, bool shouldReplaceInTable = false)
        {
            if (spriteRect.internalID != 0 && IsInternalIdInTable(spriteRect.internalID))
                return false;
            if (spriteRect.internalID == 0)
                spriteRect.internalID = SpriteRect.GenerateInternalID();

            if (shouldReplaceInTable)
            {
                if (!IsNameInTable(spriteRect.name))
                    return false;
                UpdateIdInNameIdPair(spriteRect.name, spriteRect.internalID);
            }
            else
            {
                if (IsNameInTable(spriteRect.name))
                    return false;
                AddNameFileIdPair(spriteRect.name, spriteRect.internalID);
            }

            m_SpriteRects.Add(spriteRect);
            m_NamesInUse.Add(spriteRect.name);
            return true;
        }

        public void Remove(SpriteRect spriteRect)
        {
            m_SpriteRects.Remove(spriteRect);
            m_NamesInUse.Remove(spriteRect.name);
        }

        /// <summary>
        /// Checks whether or not the name is currently in use by any of the SpriteRects in the texture.
        /// </summary>
        /// <param name="rectName">The name to check for</param>
        /// <returns>True if the name is currently in use</returns>
        public bool IsNameUsed(string rectName)
        {
            return m_NamesInUse.Contains(rectName);
        }

        /// <summary>
        /// Checks whether or not the name exists in the Name-FileId Table.
        /// </summary>
        /// <param name="rectName">The name to check for</param>
        /// <returns>True if name exists in the Name-FileId table</returns>
        public bool IsNameInTable(string rectName)
        {
            return m_SpriteNames.Contains(rectName);
        }

        /// <summary>
        /// Checks whether or not the internalId/fileId exists in the Name-FileId table.
        /// </summary>
        /// <param name="internalId">The id to check for</param>
        /// <returns>True if the id exists in the Name-FileId table</returns>
        public bool IsInternalIdInTable(long internalId)
        {
            return m_SpriteFileIds.Contains(internalId);
        }

        public List<SpriteRect> GetSpriteRects()
        {
            return m_SpriteRects;
        }

        public bool Rename(string oldName, string newName)
        {
            if (!IsNameUsed(oldName))
                return false;
            if (IsNameUsed(newName))
                return false;

            var index = m_SpriteNames.FindIndex(x => x == oldName);
            var fileId = m_SpriteFileIds[index];

            m_SpriteNames.RemoveAt(index);
            m_SpriteFileIds.RemoveAt(index);

            if (m_SpriteNames.Contains(newName))
                UpdateIdInNameIdPair(newName, fileId);
            else
                AddNameFileIdPair(newName, fileId);

            m_NamesInUse.Remove(oldName);
            m_NamesInUse.Add(newName);
            return true;
        }

        void AddNameFileIdPair(string spriteName, long fileId)
        {
            m_SpriteNames.Add(spriteName);
            m_SpriteFileIds.Add(fileId);
        }

        void UpdateIdInNameIdPair(string spriteName, long newFileId)
        {
            var index = m_SpriteNames.FindIndex(x => x == spriteName);
            m_SpriteFileIds[index] = newFileId;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {}

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            SetSpriteRects(m_SpriteRects);
        }
    }

    internal class OutlineSpriteRect : SpriteRect
    {
        public List<Vector2[]> outlines;

        public OutlineSpriteRect(SpriteRect rect)
        {
            this.name = rect.name;
            this.originalName = rect.originalName;
            this.pivot = rect.pivot;
            this.alignment = rect.alignment;
            this.border = rect.border;
            this.rect = rect.rect;
            this.spriteID = rect.spriteID;
            this.internalID = rect.internalID;
            outlines = new List<Vector2[]>();
        }
    }

    internal abstract partial class SpriteFrameModuleBase : SpriteEditorModuleBase
    {
        protected SpriteRectModel m_RectsCache;
        protected ITextureDataProvider m_TextureDataProvider;
        protected ISpriteEditorDataProvider m_SpriteDataProvider;
        protected ISpriteNameFileIdDataProvider m_NameFileIdDataProvider;
        string m_ModuleName;

        internal enum PivotUnitMode
        {
            Normalized,
            Pixels
        }

        private PivotUnitMode m_PivotUnitMode = PivotUnitMode.Normalized;

        protected SpriteFrameModuleBase(string name, ISpriteEditor sw, IEventSystem es, IUndoSystem us, IAssetDatabase ad)
        {
            spriteEditor = sw;
            eventSystem = es;
            undoSystem = us;
            assetDatabase = ad;
            m_ModuleName = name;
        }

        // implements ISpriteEditorModule

        public override void OnModuleActivate()
        {
            spriteImportMode = SpriteFrameModule.GetSpriteImportMode(spriteEditor.GetDataProvider<ISpriteEditorDataProvider>());
            m_TextureDataProvider = spriteEditor.GetDataProvider<ITextureDataProvider>();
            m_NameFileIdDataProvider = spriteEditor.GetDataProvider<ISpriteNameFileIdDataProvider>();
            m_SpriteDataProvider = spriteEditor.GetDataProvider<ISpriteEditorDataProvider>();

            int width, height;
            m_TextureDataProvider.GetTextureActualWidthAndHeight(out width, out height);
            textureActualWidth = width;
            textureActualHeight = height;

            m_RectsCache = ScriptableObject.CreateInstance<SpriteRectModel>();
            m_RectsCache.hideFlags = HideFlags.HideAndDontSave;

            var spriteList = m_SpriteDataProvider.GetSpriteRects().ToList();
            m_RectsCache.SetSpriteRects(spriteList);
            spriteEditor.spriteRects = spriteList;

            var nameFileIdPairs = m_NameFileIdDataProvider.GetNameFileIdPairs();
            m_RectsCache.SetNameFileIdPairs(nameFileIdPairs);

            if (spriteEditor.selectedSpriteRect != null)
                spriteEditor.selectedSpriteRect = m_RectsCache.spriteRects.FirstOrDefault(x => x.spriteID == spriteEditor.selectedSpriteRect.spriteID);

            AddMainUI(spriteEditor.GetMainVisualContainer());
            undoSystem.RegisterUndoCallback(UndoCallback);
        }

        public override void OnModuleDeactivate()
        {
            if (m_RectsCache != null)
            {
                undoSystem.ClearUndo(m_RectsCache);
                ScriptableObject.DestroyImmediate(m_RectsCache);
                m_RectsCache = null;
            }
            undoSystem.UnregisterUndoCallback(UndoCallback);
            RemoveMainUI(spriteEditor.GetMainVisualContainer());
        }

        public override bool ApplyRevert(bool apply)
        {
            if (apply)
            {
                var array = m_RectsCache != null ? m_RectsCache.spriteRects.ToArray() : null;
                m_SpriteDataProvider.SetSpriteRects(array);

                var spriteNames = m_RectsCache?.spriteNames;
                var spriteFileIds = m_RectsCache?.spriteFileIds;
                if (spriteNames != null && spriteFileIds != null)
                {
                    var pairList = new List<SpriteNameFileIdPair>(spriteNames.Count);
                    for (var i = 0; i < spriteNames.Count; ++i)
                        pairList.Add(new SpriteNameFileIdPair(spriteNames[i], spriteFileIds[i]));
                    m_NameFileIdDataProvider.SetNameFileIdPairs(pairList.ToArray());
                }

                var outlineDataProvider = m_SpriteDataProvider.GetDataProvider<ISpriteOutlineDataProvider>();
                var physicsDataProvider = m_SpriteDataProvider.GetDataProvider<ISpritePhysicsOutlineDataProvider>();
                foreach (var rect in array)
                {
                    if (rect is OutlineSpriteRect outlineRect)
                    {
                        if (outlineRect.outlines.Count > 0)
                        {
                            outlineDataProvider.SetOutlines(outlineRect.spriteID, outlineRect.outlines);
                            physicsDataProvider.SetOutlines(outlineRect.spriteID, outlineRect.outlines);
                        }
                    }
                }

                if (m_RectsCache != null)
                    undoSystem.ClearUndo(m_RectsCache);
            }
            else
            {
                if (m_RectsCache != null)
                {
                    undoSystem.ClearUndo(m_RectsCache);

                    var spriteList = m_SpriteDataProvider.GetSpriteRects().ToList();
                    m_RectsCache.SetSpriteRects(spriteList);

                    var nameFileIdPairs = m_NameFileIdDataProvider.GetNameFileIdPairs();
                    m_RectsCache.SetNameFileIdPairs(nameFileIdPairs);

                    spriteEditor.spriteRects = spriteList;
                    if (spriteEditor.selectedSpriteRect != null)
                        spriteEditor.selectedSpriteRect = m_RectsCache.spriteRects.FirstOrDefault(x => x.spriteID == spriteEditor.selectedSpriteRect.spriteID);
                }
            }

            return true;
        }

        public override string moduleName
        {
            get { return m_ModuleName; }
        }

        // injected interfaces
        protected IEventSystem eventSystem
        {
            get;
            private set;
        }

        protected IUndoSystem undoSystem
        {
            get;
            private set;
        }

        protected IAssetDatabase assetDatabase
        {
            get;
            private set;
        }

        protected SpriteRect selected
        {
            get { return spriteEditor.selectedSpriteRect; }
            set { spriteEditor.selectedSpriteRect = value; }
        }

        protected SpriteImportMode spriteImportMode
        {
            get; private set;
        }

        protected string spriteAssetPath
        {
            get { return assetDatabase.GetAssetPath(m_TextureDataProvider.texture); }
        }

        public bool hasSelected
        {
            get { return spriteEditor.selectedSpriteRect != null; }
        }

        public SpriteAlignment selectedSpriteAlignment
        {
            get { return selected.alignment; }
        }

        public Vector2 selectedSpritePivot
        {
            get { return selected.pivot; }
        }

        private Vector2 selectedSpritePivotInCurUnitMode
        {
            get
            {
                return m_PivotUnitMode == PivotUnitMode.Pixels
                    ? ConvertFromNormalizedToRectSpace(selectedSpritePivot, selectedSpriteRect)
                    : selectedSpritePivot;
            }
        }

        public int CurrentSelectedSpriteIndex()
        {
            if (m_RectsCache != null && selected != null)
                return m_RectsCache.FindIndex(x => x.spriteID == selected.spriteID);
            return -1;
        }

        public Vector4 selectedSpriteBorder
        {
            get { return ClampSpriteBorderToRect(selected.border, selected.rect); }
            set
            {
                undoSystem.RegisterCompleteObjectUndo(m_RectsCache, "Change Sprite Border");
                spriteEditor.SetDataModified();
                selected.border = ClampSpriteBorderToRect(value, selected.rect);
            }
        }

        public Rect selectedSpriteRect
        {
            get { return selected.rect; }
            set
            {
                undoSystem.RegisterCompleteObjectUndo(m_RectsCache, "Change Sprite rect");
                spriteEditor.SetDataModified();
                selected.rect = ClampSpriteRect(value, textureActualWidth, textureActualHeight);
            }
        }

        public string selectedSpriteName
        {
            get { return selected.name; }
            set
            {
                if (selected.name == value)
                    return;
                if (m_RectsCache.IsNameUsed(value))
                    return;
                undoSystem.RegisterCompleteObjectUndo(m_RectsCache, "Change Sprite Name");
                spriteEditor.SetDataModified();

                string oldName = selected.name;
                string newName = InternalEditorUtility.RemoveInvalidCharsFromFileName(value, true);

                // These can only be changed in sprite multiple mode
                if (string.IsNullOrEmpty(selected.originalName) && (newName != oldName))
                    selected.originalName = oldName;

                // Is the name empty?
                if (string.IsNullOrEmpty(newName))
                    newName = oldName;

                // Did the rename succeed?
                if (m_RectsCache.Rename(oldName, newName))
                    selected.name = newName;
            }
        }

        public int spriteCount
        {
            get { return m_RectsCache.spriteRects.Count; }
        }

        public Vector4 GetSpriteBorderAt(int i)
        {
            return m_RectsCache.spriteRects[i].border;
        }

        public Rect GetSpriteRectAt(int i)
        {
            return m_RectsCache.spriteRects[i].rect;
        }

        public int textureActualWidth { get; private set; }
        public int textureActualHeight { get; private set; }

        public void SetSpritePivotAndAlignment(Vector2 pivot, SpriteAlignment alignment)
        {
            undoSystem.RegisterCompleteObjectUndo(m_RectsCache, "Change Sprite Pivot");
            spriteEditor.SetDataModified();
            selected.alignment = alignment;
            selected.pivot = SpriteEditorUtility.GetPivotValue(alignment, pivot);
        }

        public bool containsMultipleSprites
        {
            get { return spriteImportMode == SpriteImportMode.Multiple; }
        }

        protected void SnapPivotToSnapPoints(Vector2 pivot, out Vector2 outPivot, out SpriteAlignment outAlignment)
        {
            Rect rect = selectedSpriteRect;

            // Convert from normalized space to texture space
            Vector2 texturePos = new Vector2(rect.xMin + rect.width * pivot.x, rect.yMin + rect.height * pivot.y);

            Vector2[] snapPoints = GetSnapPointsArray(rect);

            // Snapping is now a firm action, it will always snap to one of the snapping points.
            SpriteAlignment snappedAlignment = SpriteAlignment.Custom;
            float nearestDistance = float.MaxValue;
            for (int alignment = 0; alignment < snapPoints.Length; alignment++)
            {
                float distance = (texturePos - snapPoints[alignment]).magnitude * m_Zoom;
                if (distance < nearestDistance)
                {
                    snappedAlignment = (SpriteAlignment)alignment;
                    nearestDistance = distance;
                }
            }

            outAlignment = snappedAlignment;
            outPivot = ConvertFromTextureToNormalizedSpace(snapPoints[(int)snappedAlignment], rect);
        }

        protected void SnapPivotToPixels(Vector2 pivot, out Vector2 outPivot, out SpriteAlignment outAlignment)
        {
            outAlignment = SpriteAlignment.Custom;

            Rect rect = selectedSpriteRect;
            float unitsPerPixelX = 1.0f / rect.width;
            float unitsPerPixelY = 1.0f / rect.height;
            outPivot.x = Mathf.Round(pivot.x / unitsPerPixelX) * unitsPerPixelX;
            outPivot.y = Mathf.Round(pivot.y / unitsPerPixelY) * unitsPerPixelY;
        }

        private void UndoCallback()
        {
            UIUndoCallback();
        }

        protected static Rect ClampSpriteRect(Rect rect, float maxX, float maxY)
        {
            // Clamp rect to width height
            rect = FlipNegativeRect(rect);
            Rect newRect = new Rect();

            newRect.xMin = Mathf.Clamp(rect.xMin, 0, maxX - 1);
            newRect.yMin = Mathf.Clamp(rect.yMin, 0, maxY - 1);
            newRect.xMax = Mathf.Clamp(rect.xMax, 1, maxX);
            newRect.yMax = Mathf.Clamp(rect.yMax, 1, maxY);

            // Prevent width and height to be 0 value after clamping.
            if (Mathf.RoundToInt(newRect.width) == 0)
                newRect.width = 1;
            if (Mathf.RoundToInt(newRect.height) == 0)
                newRect.height = 1;

            return SpriteEditorUtility.RoundedRect(newRect);
        }

        protected static Rect FlipNegativeRect(Rect rect)
        {
            Rect newRect = new Rect();

            newRect.xMin = Mathf.Min(rect.xMin, rect.xMax);
            newRect.yMin = Mathf.Min(rect.yMin, rect.yMax);
            newRect.xMax = Mathf.Max(rect.xMin, rect.xMax);
            newRect.yMax = Mathf.Max(rect.yMin, rect.yMax);

            return newRect;
        }

        protected static Vector4 ClampSpriteBorderToRect(Vector4 border, Rect rect)
        {
            Rect flipRect = FlipNegativeRect(rect);
            float w = flipRect.width;
            float h = flipRect.height;

            Vector4 newBorder = new Vector4();

            // Make sure borders are within the width/height and left < right and top < bottom
            newBorder.x = Mathf.RoundToInt(Mathf.Clamp(border.x, 0, Mathf.Min(Mathf.Abs(w - border.z), w))); // Left
            newBorder.z = Mathf.RoundToInt(Mathf.Clamp(border.z, 0, Mathf.Min(Mathf.Abs(w - newBorder.x), w))); // Right

            newBorder.y = Mathf.RoundToInt(Mathf.Clamp(border.y, 0, Mathf.Min(Mathf.Abs(h - border.w), h))); // Bottom
            newBorder.w = Mathf.RoundToInt(Mathf.Clamp(border.w, 0, Mathf.Min(Mathf.Abs(h - newBorder.y), h))); // Top

            return newBorder;
        }
    }
}
