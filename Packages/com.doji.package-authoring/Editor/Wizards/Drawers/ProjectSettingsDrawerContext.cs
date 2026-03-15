using System;
using System.Collections.Generic;

namespace Doji.PackageAuthoring.Editor.Wizards {
    /// <summary>
    /// Carries host-specific rendering options for <c>ProjectSettings</c> property drawers.
    /// </summary>
    internal static class ProjectSettingsDrawerContext {
        private static readonly Stack<State> States = new();

        /// <summary>
        /// Active drawer options for the current rendering scope.
        /// </summary>
        public static State Current => States.Count > 0 ? States.Peek() : State.Default;

        /// <summary>
        /// Pushes a temporary rendering scope for a project-settings block.
        /// </summary>
        public static IDisposable Push(string productLabel, bool includeTargetLocation) {
            States.Push(new State(productLabel, includeTargetLocation));
            return new Scope();
        }

        /// <summary>
        /// Immutable drawer configuration for a single rendering scope.
        /// </summary>
        internal readonly struct State {
            public static State Default => new("Product Name", true);

            public State(string productLabel, bool includeTargetLocation) {
                ProductLabel = string.IsNullOrWhiteSpace(productLabel) ? "Product Name" : productLabel;
                IncludeTargetLocation = includeTargetLocation;
            }

            public string ProductLabel { get; }

            public bool IncludeTargetLocation { get; }
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
