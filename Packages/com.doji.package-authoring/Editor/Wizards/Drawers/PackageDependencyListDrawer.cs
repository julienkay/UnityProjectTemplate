using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Wizards.Models;
using Doji.PackageAuthoring.Editor.Wizards.PackageSearch;

namespace Doji.PackageAuthoring.Editor.Wizards {
    /// <summary>
    /// Draws the serialized package-dependency collection with autocomplete-backed dependency rows.
    /// </summary>
    [CustomPropertyDrawer(typeof(PackageDependencyList))]
    internal sealed class PackageDependencyListDrawer : PropertyDrawer {
        private static readonly string ItemsField = $"<{nameof(PackageDependencyList.Items)}>k__BackingField";

        private static readonly string DependencyPackageNameField =
            $"<{nameof(PackageDependencyEntry.PackageName)}>k__BackingField";

        private static readonly string DependencyVersionField =
            $"<{nameof(PackageDependencyEntry.Version)}>k__BackingField";

        private static readonly Dictionary<string, State> States = new();

        private static class Styles {
            public static readonly GUIContent Version =
                EditorGUIUtility.TrTextContent("Version", "Must follow SemVer (ex: 1.0.0-preview.1).");

            public static readonly GUIContent Package =
                EditorGUIUtility.TrTextContent("Package name", "Must be lowercase");
        }

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            State state = GetState(property);
            state.OverflowMode = PackageSettingsDrawerContext.Current.OverflowMode;
            return state.DependenciesList.GetHeight();
        }

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            State state = GetState(property);
            state.OverflowMode = PackageSettingsDrawerContext.Current.OverflowMode;
            Rect listRect = new(position.x, position.y, position.width, state.DependenciesList.GetHeight());

            EditorGUI.BeginProperty(position, label, property);
            state.DependenciesList.DoList(listRect);
            EditorGUI.EndProperty();
        }

        private static State GetState(SerializedProperty property) {
            string key = $"{property.serializedObject.targetObject.GetInstanceID()}::{property.propertyPath}";
            if (!States.TryGetValue(key, out State state)) {
                state = new State();
                States[key] = state;
            }

            state.Bind(property);
            return state;
        }

        private sealed class State {
            private SerializedObject _serializedObject;
            private SerializedProperty _itemsProperty;
            private string _propertyPath;

            public State() {
                DependencyAutocompleteField = new UnityRegistryPackageAutocompleteField(
                    InternalEditorUtility.RepaintAllViews,
                    overflowMode: UnityRegistryPackageAutocompleteField.SuggestionOverflowMode.Scroll);
                PackageSearchCache.Shared.EnsureLoaded();
            }

            public ReorderableList DependenciesList { get; private set; }

            public UnityRegistryPackageAutocompleteField DependencyAutocompleteField { get; }

            public UnityRegistryPackageAutocompleteField.SuggestionOverflowMode OverflowMode { get; set; } =
                UnityRegistryPackageAutocompleteField.SuggestionOverflowMode.Scroll;

            public void Bind(SerializedProperty property) {
                DependencyAutocompleteField.SetOverflowMode(OverflowMode);
                if (_serializedObject == property.serializedObject && _propertyPath == property.propertyPath) {
                    return;
                }

                _serializedObject = property.serializedObject;
                _itemsProperty = property.FindPropertyRelative(ItemsField);
                _propertyPath = property.propertyPath;
                DependenciesList = new ReorderableList(
                    _serializedObject,
                    _itemsProperty,
                    draggable: true,
                    displayHeader: true,
                    displayAddButton: true,
                    displayRemoveButton: true) {
                    drawHeaderCallback = DrawDependencyHeader,
                    drawElementCallback = DrawDependencyElement,
                    elementHeightCallback = GetDependencyElementHeight
                };
            }

            private void DrawDependencyHeader(Rect rect) {
                float width = rect.width;
                rect.x += 4f;
                rect.width = width / 3f * 2f - 2f;
                GUI.Label(rect, Styles.Package, EditorStyles.label);

                rect.x += width / 3f * 2f;
                rect.width = width / 3f - 4f;
                GUI.Label(rect, Styles.Version, EditorStyles.label);
            }

            private void DrawDependencyElement(Rect rect, int index, bool isActive, bool isFocused) {
                SerializedProperty dependency = DependenciesList.serializedProperty.GetArrayElementAtIndex(index);
                SerializedProperty packageNameProperty = dependency.FindPropertyRelative(DependencyPackageNameField);
                SerializedProperty versionProperty = dependency.FindPropertyRelative(DependencyVersionField);
                bool shouldShowSuggestions = ShouldShowSuggestions(index, packageNameProperty);
                DependencyAutocompleteField.Draw(
                    rect,
                    $"{_serializedObject.targetObject.GetInstanceID()}.{_propertyPath}.dependency.{index}",
                    packageNameProperty,
                    versionProperty,
                    shouldShowSuggestions);
            }

            private float GetDependencyElementHeight(int index) {
                SerializedProperty dependency = DependenciesList.serializedProperty.GetArrayElementAtIndex(index);
                SerializedProperty packageNameProperty = dependency.FindPropertyRelative(DependencyPackageNameField);
                bool shouldShowSuggestions = ShouldShowSuggestions(index, packageNameProperty);
                return DependencyAutocompleteField.GetHeight(
                    $"{_serializedObject.targetObject.GetInstanceID()}.{_propertyPath}.dependency.{index}",
                    packageNameProperty.stringValue,
                    shouldShowSuggestions);
            }

            private bool ShouldShowSuggestions(int index, SerializedProperty packageNameProperty) {
                // Height calculation and row drawing must use the same stable predicate or the
                // ReorderableList can reserve one height during Layout and draw another during Repaint.
                // The selected row index is stable enough as long as the backing target object is stable too.
                if (index != DependenciesList.index) {
                    return false;
                }

                string packageName = packageNameProperty.stringValue?.Trim();
                if (string.IsNullOrWhiteSpace(packageName)) {
                    return false;
                }

                // Once the current value already resolves to a known package, keep the row compact
                // until the user changes it again instead of permanently pinning the suggestion list open.
                return !PackageSearchCache.Shared.FindExact(packageName).HasValue;
            }
        }
    }
}
