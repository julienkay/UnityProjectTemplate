namespace Doji.PackageAuthoring.Editor.Wizards.Models {
    /// <summary>
    /// Supported generated license templates for new packages.
    /// </summary>
    public enum LicenseType {
        /// <summary>
        /// Generates an MIT license file.
        /// </summary>
        MIT,

        /// <summary>
        /// Generates an Apache 2.0 license file.
        /// </summary>
        Apache,

        /// <summary>
        /// Generates a BSD-style license file.
        /// </summary>
        BSD
    }
}
