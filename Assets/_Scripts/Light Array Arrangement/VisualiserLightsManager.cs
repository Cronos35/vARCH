using System.Collections;
using UnityEngine;

namespace Assets._Scripts
{
    public class VisualiserLightsManager : MonoBehaviour
    {
        public bool overrideBlockSettings;

        [SerializeField] private VisualiserLightBlock visualiserLightBlock;
        [Header("Light Settings")]
        public LightObject _lightObject;
        [Header("Light counts")]
        public int _lightBlockCount = 1;
        //load these values onto the visualiser manager when instantiating
        public int _lightArraysPerBlock = 1;
        public int _lightRowsPerArray = 1;
        public int _lightsPerRow = 1;

        [Header("Light Block Transforms")]
        public float _lightBlockSideSpacing = 1;
        public float _lightBlockVerticalSpacing = 1;
        public int _lightBlockForeAftOffset = 1;

        [Header("Arrangement Settings")]
        public LightArrayOrientation _arrayOrientation;
        public LightArrayOrientation _lightOrientation;
        public LightArrangementShape _arrangementShape;

        [Header("Light block properties")]
        [Header("Spacing and offset")]
        public float _lightArraySpacing = 1;
        public float _lightRowSpacing = 1;
        public float _singleLightSpacing = 1;

        public float _diagonalOffset = 1;
        public float _horizontalOffset = 1;
        public float _rowVerticalOffset = 1;
        
        [Header("Rotations")]
        public Vector3 _lightRotation = Vector3.right;
        public Vector3 _oddLightsRotation = Vector3.right;

        [Header("Circular and cone arrangement")]
        public bool _enableRotation;
        public LightArrayDirection _rotationDirection;
        public LightArrayDirection _lightTriggerDirection;
        public float _minRotationSpeed = 1;
        public float _maxRotationSpeed = 2;
        public float _radius = 1;
        [Range(1, 10)] public float _coneTightnessFactor = 1;
        [Range(0, 360)] public float _arc;

        [Header("Colors")]
        public Color _primaryColor;
        public Color _secondaryColor;
        public Color _tertiaryColor;
        public ColorAssignmentMode _colorAssignmentMode;

        [Header("Stage Preset Data")]
        public StagePreset _stagePreset;
        [SerializeField] private EventSystem eventSystem;
        [Space]
        public SpectrumRangeAssignment _spectrumRangeAssignment;
        public VisualiserLightBlock[] VisualiserLightBlocks { get; set; }

        private void Awake()
        {
            VisualiserLightBlocks = new VisualiserLightBlock[_lightBlockCount];
            for (int i = 0; i < _lightBlockCount; i++)
            {
                VisualiserLightBlocks[i] = transform.GetChild(i).GetComponent<VisualiserLightBlock>();
            }
        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        public void InitializeLightBlocks()
        {
            RemoveAll();
            VisualiserLightBlocks = new VisualiserLightBlock[_lightBlockCount];
            LightBlockProperties lightArrayProps = InitializeLightBlockProperties();

            for (int i = 0; i < _lightBlockCount; i++)
            {
                Transform newlightBlock = Instantiate(visualiserLightBlock).transform;
                newlightBlock.SetParent(transform);

                Vector3 lightBlockPosition = new Vector3(0, 0, i * _lightBlockSideSpacing);

                newlightBlock.localPosition = lightBlockPosition;
                VisualiserLightBlocks[i] = newlightBlock.GetComponent<VisualiserLightBlock>();

                VisualiserLightBlocks[i].InitializeArrayProperties(lightArrayProps);
            }
        }

        private LightBlockProperties InitializeLightBlockProperties()
        {
            ArraySpacingProperties spacingProperties = new ArraySpacingProperties(_lightArraySpacing, _lightRowSpacing, _singleLightSpacing);
            LightArrayCounts lightArrayCounts = new LightArrayCounts(_lightArraysPerBlock, _lightRowsPerArray, _lightsPerRow);
            ArrayOffsets arrayOffsets = new ArrayOffsets(_diagonalOffset, _horizontalOffset, _rowVerticalOffset);
            LightRotations lightRotations = new LightRotations(_lightRotation, _oddLightsRotation);
            CircularArrangementData circularArrangementData = new CircularArrangementData(_radius, _coneTightnessFactor, _arc, _enableRotation, _rotationDirection, _minRotationSpeed, _maxRotationSpeed);
            LightColors lightColors = new LightColors(_primaryColor, _secondaryColor, _tertiaryColor, _colorAssignmentMode);
            ArrangementData arrangementData = new ArrangementData(_arrayOrientation, _lightOrientation, _arrangementShape);

            return new LightBlockProperties(spacingProperties, lightArrayCounts, arrayOffsets, lightRotations, circularArrangementData, _lightObject, lightColors, arrangementData, _lightTriggerDirection, _spectrumRangeAssignment, new Vector3(), new Quaternion());
        }

        public void SetLightBlockTransforms()
        {
            if(VisualiserLightBlocks == null)
            {
                return;
            }
            if(VisualiserLightBlocks.Length == 0)
            {
                return;
            }

            for (int i = 0; i < VisualiserLightBlocks.Length; i++)
            {
                if (VisualiserLightBlocks.Length <= 0)
                {
                    return;
                }
                Transform currentLightBlock = VisualiserLightBlocks[i].transform;
                
                Vector3 lightBlockPosition = new Vector3(_lightBlockForeAftOffset, 0, (i * _lightBlockSideSpacing) - (_lightBlockSideSpacing * (_lightBlockCount - 1) / 2));
                //if (i < VisualiserLightBlocks.Length / 2)
                //{
                //    lightBlockPosition.z *= -1; 
                //}

                VisualiserLightBlocks[i]._lightArraySpacing = _lightArraySpacing;
                VisualiserLightBlocks[i].ArrangeLights();
                currentLightBlock.localPosition = lightBlockPosition;
            }
        }

        public void SaveLightBlockData(StagePreset stage)
        {
            stage.lightBlockProperties = new LightBlockProperties[VisualiserLightBlocks.Length];

            for (int i = 0; i < VisualiserLightBlocks.Length; i++)
            {
                _stagePreset.lightBlockProperties[i] = VisualiserLightBlocks[i].BlockProperties;
            }
        }

        public void SetColors()
        {
            for (int i = 0; i < VisualiserLightBlocks.Length; i++)
            {
                VisualiserLightBlocks[i]._primaryColor = _primaryColor;
                VisualiserLightBlocks[i]._secondaryColor = _secondaryColor;
                VisualiserLightBlocks[i]._tertiaryColor = _tertiaryColor;
                VisualiserLightBlocks[i]._colorAssignmentMode = _colorAssignmentMode;
                VisualiserLightBlocks[i].SetColors();
            }
        }
        public void RemoveAll()
        {
            VisualiserLightBlocks = new VisualiserLightBlock[0];
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        public void SetLightArrangementSettings()
        {
            for (int i = 0; i < VisualiserLightBlocks.Length; i++)
            {
                VisualiserLightBlocks[i]._arrayOrientation = _arrayOrientation;
                VisualiserLightBlocks[i]._lightOrientation = _lightOrientation;
                VisualiserLightBlocks[i]._arrangementShape = _arrangementShape;
            }
        }

        public void LoadLightBlockPreset()
        {
            RemoveAll();
            VisualiserLightBlocks = new VisualiserLightBlock[_stagePreset.lightBlockProperties.Length];

            for (int i = 0; i < _stagePreset.lightBlockProperties.Length; i++)
            {
                LightBlockProperties lightArrayProps = _stagePreset.lightBlockProperties[i] ;

                Transform newlightBlock = Instantiate(visualiserLightBlock).transform;
                newlightBlock.SetParent(transform);

                VisualiserLightBlocks[i] = newlightBlock.GetComponent<VisualiserLightBlock>();

                VisualiserLightBlocks[i].InitializeArrayProperties(lightArrayProps);

            }
        }
    }
}