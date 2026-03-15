using System;
using System.Collections.Generic;
using Doji.PackageAuthoring.Editor.Wizards.PackageSearch;

namespace Doji.PackageAuthoring.Editor.Wizards {
    /// <summary>
    /// Carries host-specific rendering options for <c>PackageSettings</c> property drawers.
    /// </summary>
    internal static class PackageSettingsDrawerContext {
        private static readonly Stack<State> States = new();

        /// <summary>
        /// Active drawer options for the current rendering scope.
        /// </summary>
        public static State Current => States.Count > 0 ? States.Peek() : State.Default;

        /// <summary>
        /// Pushes a temporary rendering scope for a package-settings block.
        /// </summary>
        public static IDisposable Push(UnityRegistryPackageAutocompleteField.SuggestionOverflowMode overflowMode) {
            States.Push(new State(overflowMode));
            return new Scope();
        }

        /// <summary>
        /// Immutable drawer configuration for a single rendering scope.
        /// </summary>
        internal readonly struct State {
            public static State Default => new(
                UnityRegistryPackageAutocompleteField.SuggestionOverflowMode.Scroll);

            public State(UnityRegistryPackageAutocompleteField.SuggestionOverflowMode overflowMode) {
                OverflowMode = overflowMode;
            }

            public UnityRegistryPackageAutocompleteField.SuggestionOverflowMode OverflowMode { get; }
        }

        private readonly struct Scope : IDisposable {
            public void Dispose() {
                if (States.Count > 0) {
                    States.Pop();
                }
            }
        }
    }
}
