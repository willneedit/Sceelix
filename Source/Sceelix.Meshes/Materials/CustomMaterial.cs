using Sceelix.Core.Attributes;
using Sceelix.Core.Extensions;

namespace Sceelix.Meshes.Materials
{
    public class CustomMaterial : MeshMaterial
    {
        private static readonly AttributeKey ShaderNameKey = new GlobalAttributeKey("ShaderName");

        public CustomMaterial(string shaderName)
        {
            ShaderName = shaderName;
        }

        public string ShaderName
        {
            get { return this.GetAttribute<string>(ShaderNameKey); }
            set { this.SetAttribute(ShaderNameKey, value); }
        }
    }
}