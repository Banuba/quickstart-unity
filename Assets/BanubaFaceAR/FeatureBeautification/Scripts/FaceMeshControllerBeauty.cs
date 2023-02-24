namespace BNB.Beautification
{
    public class FaceMeshControllerBeauty : FaceMeshController
    {
        public void SetCoeffParam(string paramName, float value)
        {
            _meshMaterial.SetFloat(paramName, value);
        }

        public void SetBeautyFeatureEnabled(string paramName, int value)
        {
            _meshMaterial.SetInt(paramName, value);
        }
    }
}
