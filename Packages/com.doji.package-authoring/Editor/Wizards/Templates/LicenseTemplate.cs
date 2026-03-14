using Doji.PackageAuthoring.Editor.Wizards.Models;

namespace Doji.PackageAuthoring.Editor.Wizards {
    public partial class PackageCreationWizard {
        public string GetLicense() {
            switch (_packageSettings.LicenseType) {
                case LicenseType.Apache:
                    return GetApacheLicense();
                case LicenseType.BSD:
                    return GetBSDLicense();
                case LicenseType.MIT:
                default:
                    return GetMITLicense();
            }
        }

        private string GetMITLicense() {
            string copyrightHolder = _packageSettings.IncludeAuthor && !string.IsNullOrWhiteSpace(_packageSettings.Author)
                ? _packageSettings.Author
                : _projectSettings.CompanyName;

            return $@"MIT License

Copyright (c) 2025 {copyrightHolder}

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
";
        }

        private static string GetApacheLicense() {
            throw new System.NotImplementedException();
        }

        private static string GetBSDLicense() {
            throw new System.NotImplementedException();
        }
    }
}
