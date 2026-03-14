using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Doji.PackageAuthoring.Editor {
    /// <summary>
    /// Reusable editor field for editing a package dependency pair with merged package-source suggestions.
    /// </summary>
    public sealed class UnityRegistryPackageAutocompleteField : IDisposable {
        private const int MaxVisibleSuggestions = 6;
        private const float SourceBadgeWidth = 64f;
        private const float SourceBadgePadding = 6f;

        /// <summary>
        /// Controls how the autocomplete field behaves when more matches exist than are shown initially.
        /// </summary>
        public enum SuggestionOverflowMode {
            /// <summary>
            /// Shows the first page of matches and appends a trailing hint that more results are available.
            /// </summary>
            Hint,

            /// <summary>
            /// Shows matches inside a fixed-height scroll view so additional results remain accessible inline.
            /// </summary>
            Scroll
        }

        private readonly Action _requestRepaint;
        private readonly PackageSearchCache _cache;
        private readonly SuggestionOverflowMode _overflowMode;

        private string _activeFieldKey;
        private readonly Dictionary<string, Vector2> _scrollPositions = new();
        private static readonly Color BadgeTextColor = new(1f, 1f, 1f, 0.98f);
        private static GUIStyle _badgeLabelStyle;

        public UnityRegistryPackageAutocompleteField(
            Action requestRepaint,
            PackageSearchCache cache = null,
            SuggestionOverflowMode overflowMode = SuggestionOverflowMode.Hint) {
            _requestRepaint = requestRepaint;
            _cache = cache ?? PackageSearchCache.Shared;
            _overflowMode = overflowMode;
            _cache.Changed += HandleCacheChanged;
        }

        public void Dispose() {
            _cache.Changed -= HandleCacheChanged;
        }

        public float GetHeight(string fieldKey, string packageName) {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float height = lineHeight;

            if (!ShouldShowSuggestions(fieldKey, packageName)) {
                return height + spacing;
            }

            if (_cache.IsLoading && !_cache.HasPackages) {
                return height + spacing + lineHeight + spacing;
            }

            List<PackageSearchEntry> matches = _cache.GetMatches(packageName, MaxVisibleSuggestions);
            if (matches.Count == 0) {
                return height + spacing;
            }

            bool hasMoreMatches = _cache.HasMoreMatches(packageName, matches.Count);
            float suggestionsHeight = matches.Count * (lineHeight + spacing);
            if (_overflowMode == SuggestionOverflowMode.Scroll && hasMoreMatches) {
                suggestionsHeight = MaxVisibleSuggestions * (lineHeight + spacing);
            }
            else if (hasMoreMatches) {
                suggestionsHeight += lineHeight + spacing;
            }

            return height + spacing + suggestionsHeight;
        }

        public void Draw(
            Rect rect,
            string fieldKey,
            SerializedProperty packageNameProperty,
            SerializedProperty versionProperty) {
            _cache.EnsureLoaded();

            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float width = rect.width;

            Rect packageRect = new(rect.x + 4, rect.y, width / 3 * 2 - 2, lineHeight);
            Rect versionRect = new(rect.x + 4 + width / 3 * 2, rect.y, width / 3 - 4, lineHeight);

            string controlName = GetPackageControlName(fieldKey);
            GUI.SetNextControlName(controlName);
            string newPackageName = EditorGUI.TextField(packageRect, packageNameProperty.stringValue);
            if (newPackageName != packageNameProperty.stringValue) {
                packageNameProperty.stringValue = newPackageName;
                _activeFieldKey = fieldKey;
                TryApplySuggestedVersion(packageNameProperty, versionProperty);
            }
            else if (GUI.GetNameOfFocusedControl() == controlName) {
                _activeFieldKey = fieldKey;
            }

            versionProperty.stringValue = EditorGUI.TextField(versionRect, versionProperty.stringValue);

            if (!ShouldShowSuggestions(fieldKey, packageNameProperty.stringValue)) {
                return;
            }

            if (_cache.IsLoading && !_cache.HasPackages) {
                Rect statusRect = new(rect.x + 4, rect.y + lineHeight + spacing, rect.width - 8, lineHeight);
                EditorGUI.LabelField(statusRect, _cache.StatusMessage, EditorStyles.miniLabel);
                return;
            }

            List<PackageSearchEntry> matches = _cache.GetMatches(packageNameProperty.stringValue, MaxVisibleSuggestions);
            if (matches.Count == 0) {
                return;
            }

            bool hasMoreMatches = _cache.HasMoreMatches(packageNameProperty.stringValue, matches.Count);
            if (_overflowMode == SuggestionOverflowMode.Scroll && hasMoreMatches) {
                DrawScrollableSuggestions(rect, fieldKey, lineHeight, spacing, packageNameProperty, versionProperty);
                return;
            }

            float suggestionY = rect.y + lineHeight + spacing;
            foreach (var packageInfo in matches) {
                Rect buttonRect = new(rect.x + 4, suggestionY, rect.width - 8, lineHeight);
                if (DrawSuggestionRow(buttonRect, packageInfo)) {
                    packageNameProperty.stringValue = packageInfo.PackageName;
                    if (!string.IsNullOrWhiteSpace(packageInfo.Version)) {
                        versionProperty.stringValue = packageInfo.Version;
                    }

                    _activeFieldKey = null;
                    GUI.FocusControl(null);
                }

                suggestionY += lineHeight + spacing;
            }

            if (hasMoreMatches) {
                Rect overflowRect = new(rect.x + 4, suggestionY, rect.width - 8, lineHeight);
                EditorGUI.LabelField(overflowRect, "...more matches available", EditorStyles.miniLabel);
            }
        }

        private bool ShouldShowSuggestions(string fieldKey, string packageName) {
            return _activeFieldKey == fieldKey
                   && IsPackageFieldFocused(fieldKey)
                   && !string.IsNullOrWhiteSpace(packageName);
        }

        private static string GetPackageControlName(string fieldKey) {
            return $"{fieldKey}.package";
        }

        private static bool IsPackageFieldFocused(string fieldKey) {
            return GUI.GetNameOfFocusedControl() == GetPackageControlName(fieldKey);
        }

        private void DrawScrollableSuggestions(
            Rect rect,
            string fieldKey,
            float lineHeight,
            float spacing,
            SerializedProperty packageNameProperty,
            SerializedProperty versionProperty) {
            List<PackageSearchEntry> allMatches =
                _cache.GetMatches(packageNameProperty.stringValue, int.MaxValue);

            float viewportHeight = MaxVisibleSuggestions * (lineHeight + spacing);
            Rect viewRect = new(rect.x + 4, rect.y + lineHeight + spacing, rect.width - 8, viewportHeight);
            float contentHeight = allMatches.Count * (lineHeight + spacing);
            Rect contentRect = new(0, 0, viewRect.width - 16, contentHeight);

            Vector2 scrollPosition = _scrollPositions.TryGetValue(fieldKey, out var currentPosition)
                ? currentPosition
                : Vector2.zero;

            scrollPosition = GUI.BeginScrollView(viewRect, scrollPosition, contentRect);

            float suggestionY = 0f;
            foreach (var packageInfo in allMatches) {
                Rect buttonRect = new(0, suggestionY, contentRect.width, lineHeight);
                if (DrawSuggestionRow(buttonRect, packageInfo)) {
                    packageNameProperty.stringValue = packageInfo.PackageName;
                    if (!string.IsNullOrWhiteSpace(packageInfo.Version)) {
                        versionProperty.stringValue = packageInfo.Version;
                    }

                    _activeFieldKey = null;
                    GUI.FocusControl(null);
                }

                suggestionY += lineHeight + spacing;
            }

            GUI.EndScrollView();
            _scrollPositions[fieldKey] = scrollPosition;
        }

        private static bool DrawSuggestionRow(Rect rect, PackageSearchEntry packageInfo) {
            Event currentEvent = Event.current;
            bool isHovered = rect.Contains(currentEvent.mousePosition);

            if (currentEvent.type == EventType.Repaint) {
                EditorStyles.miniButton.Draw(rect, GUIContent.none, isHovered, false, false, false);
            }

            Rect badgeRect = new(
                rect.xMax - SourceBadgeWidth - SourceBadgePadding,
                rect.y + 1f,
                SourceBadgeWidth,
                rect.height - 2f);

            Rect textRect = new(
                rect.x + 6f,
                rect.y,
                badgeRect.xMin - rect.x - 10f,
                rect.height);

            string versionLabel = string.IsNullOrWhiteSpace(packageInfo.Version) ? "version unavailable" : packageInfo.Version;
            EditorGUI.LabelField(textRect, $"{packageInfo.PackageName}  {versionLabel}", EditorStyles.miniLabel);
            DrawSourceBadge(badgeRect, packageInfo.SourceName);

            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && rect.Contains(currentEvent.mousePosition)) {
                currentEvent.Use();
                GUI.changed = true;
                return true;
            }

            return false;
        }

        private static void DrawSourceBadge(Rect rect, string sourceName) {
            Color originalColor = GUI.color;
            GUI.color = GetSourceBadgeColor(sourceName);
            GUI.Box(rect, GUIContent.none, EditorStyles.miniButton);
            GUI.color = originalColor;

            Color originalContentColor = GUI.contentColor;
            GUI.contentColor = BadgeTextColor;
            EditorGUI.LabelField(rect, sourceName, BadgeLabelStyle);
            GUI.contentColor = originalContentColor;
        }

        private static GUIStyle BadgeLabelStyle {
            get {
                _badgeLabelStyle ??= new GUIStyle(EditorStyles.miniBoldLabel) {
                    alignment = TextAnchor.MiddleCenter,
                    clipping = TextClipping.Clip
                };

                return _badgeLabelStyle;
            }
        }

        private static Color GetSourceBadgeColor(string sourceName) {
            if (string.Equals(sourceName, "Unity", StringComparison.OrdinalIgnoreCase)) {
                return new Color(0.76f, 0.82f, 0.9f, 1f);
            }

            uint hash = ComputeStableHash(sourceName);
            float hue = (hash % 1000u) / 1000f;
            return Color.HSVToRGB(hue, 0.18f, 0.9f);
        }

        private static uint ComputeStableHash(string value) {
            if (string.IsNullOrEmpty(value)) {
                return 0;
            }

            unchecked {
                const uint fnvOffsetBasis = 2166136261u;
                const uint fnvPrime = 16777619u;

                uint hash = fnvOffsetBasis;
                foreach (char character in value) {
                    hash ^= character;
                    hash *= fnvPrime;
                }

                return hash;
            }
        }

        private void TryApplySuggestedVersion(SerializedProperty packageNameProperty, SerializedProperty versionProperty) {
            PackageSearchEntry? exactMatch = _cache.FindExact(packageNameProperty.stringValue);
            if (!exactMatch.HasValue || string.IsNullOrWhiteSpace(exactMatch.Value.Version)) {
                return;
            }

            versionProperty.stringValue = exactMatch.Value.Version;
        }

        private void HandleCacheChanged() {
            _requestRepaint?.Invoke();
        }
    }
}
