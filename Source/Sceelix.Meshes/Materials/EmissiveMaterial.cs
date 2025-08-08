using Sceelix.Core.Attributes;
using Sceelix.Core.Extensions;
using Sceelix.Mathematics.Data;

namespace Sceelix.Meshes.Materials
{
    /// <summary>
    /// Why material is generic:
    /// - Easier to create derivatives, because
    ///    - No need to implement equality comparer
    ///    - No need to implement serializer
    /// </summary>
    public class EmissiveMaterial : MeshMaterial
    {
        private static readonly AttributeKey ColorKey = new GlobalAttributeKey("DefaultColor");

        private static readonly AttributeKey IntensityKey = new GlobalAttributeKey("Intensity");

        public EmissiveMaterial(Color defaultColor, float intensity)
        {
            DefaultColor = defaultColor;
            Intensity = intensity;
        }



        public Color DefaultColor
        {
            get { return this.GetAttribute<Color>(ColorKey); }
            set { this.SetAttribute(ColorKey, value); }
        }

        public float Intensity
        {
            get { return this.GetAttribute<float>(IntensityKey); }
            set { this.SetAttribute(IntensityKey, value); }
        }
    }
}