using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Assets._Scripts
{
    //responsible for arranging lights, 
    public class VisualiserLightBlock : MonoBehaviour
    {
        [SerializeField] private EventSystem eventSystem;
        [Header("Colors")]
        public Color _primaryColor;
        public Color _secondaryColor;
        public Color _tertiaryColor;
        public Color[] _lightColors;
        public ColorAssignmentMode _colorAssignmentMode;
        [SerializeField] private LightArrayDirection _lightTriggerDirection;
        [SerializeField] private LightArrayDirection _arrayRotation;
        [SerializeField] private SpectrumRangeAssignment _rangeAssignment;

        [Header("Light Settings")]
        public LightArrayOrientation _arrayOrientation;
        public LightArrayOrientation _lightOrientation;
        public LightArrangementShape _arrangementShape; 
        
        [Header("Prefabs")]
        public GameObject _lightArrayController;
        public GameObject _lightRowController;
        public GameObject[] _lightPrefabs;
        public LightObject _selectedLightType;

        [Header("Light counts")]
        public int _lightArrayControllerCount;
        public int _lightRowsPerController;
        public int _lightsPerRow;
        
        [Header("Light transforms")]
        [Header("Spacing and offset")]
        public float _lightArraySpacing = 1;
        public float _lightRowSpacing = 1;
        public float _singleLightSpacing = 1;
        public float _diagonalOffset = 1;
        public float _horizontalOffset = 1;
        public float _rowVerticalOffset = 1;
        [Header("Rotations")]
        public Vector3 _lightRotation = Vector3.one;
        public Vector3 _oddLightsRotation = Vector3.one;
        [Header("Circular and cone arrangement")]
        public float _radius = 1;
        [Range(1, 10)]  public float _coneTightnessFactor = 1;
        [Range(0, 360)] public float _arc;
        [Header("Rotation Properties")]
        public bool _staggeredRotationSpeed;
        public bool _enableRotation;
        public LightArrayDirection _rotationDirection;
        public float _minRotationSpeed;
        public float _arrayRotationSpeed;
        [Space]
        private Camera eqBarsCamera;
        public Transform[] ArrayTransforms { get; set; }
        public Transform[] LightRowTransforms { get; set; }
        public Transform[] LightTransforms { get; set; }
        private LightArrayController[] lightArrays;

        public GameObject lightObject { get; set; }
        private LightBlockProperties _blockProperties;
        public LightBlockProperties BlockProperties { get { return _blockProperties; } set { _blockProperties = value; } }
        // Use this for initialization
        // instantiate visualser cubes here
        // initialize the values of the visualiser cubes here
        private void Awake()
        {
            eventSystem._onEmitCamera += SetCamera;
            lightObject = _lightPrefabs[0];

            GetLightArrays();
        }

        private void GetLightArrays()
        {
            lightArrays = new LightArrayController[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                lightArrays[i] = transform.GetChild(i).GetComponent<LightArrayController>();
            }
        }

        private void OnEnable()
        {
            eventSystem._onUpdateStageColors += LoadLightColors;   
        }

        private void OnDisable()
        {
            eventSystem._onUpdateStageColors -= LoadLightColors;
        }

        private void LoadLightColors(LightColors lightColors)
        {
            _primaryColor = lightColors.primaryColor;
            _secondaryColor = lightColors.secondaryColor;
            _tertiaryColor = lightColors.tertiaryColor;

            SetColors();
        }

        private void SetCamera(Camera camera)
        {
            eqBarsCamera = camera;
            eventSystem._onEmitCamera -= SetCamera;
        }

        #region methods
        public void InitializeArrayProperties(LightBlockProperties lightBlockProperties)
        {
            BlockProperties = lightBlockProperties;

            _lightArraySpacing = lightBlockProperties._spacingProperties.lightArraySpacing;
            _lightRowSpacing = lightBlockProperties._spacingProperties.lightRowSpacing;
            _singleLightSpacing = lightBlockProperties._spacingProperties.singleLightSpacing;

            _diagonalOffset = lightBlockProperties._offsets.diagonalOffset;
            _horizontalOffset = lightBlockProperties._offsets.horizontalOffset;
            _rowVerticalOffset = lightBlockProperties._offsets.rowVerticalOffset;

            _lightRotation = lightBlockProperties._rotations.lightRotation;
            _oddLightsRotation = lightBlockProperties._rotations.oddLightsRotation;

            _lightArrayControllerCount = lightBlockProperties._arrayCounts.lightArrayCount;
            _lightRowsPerController = lightBlockProperties._arrayCounts.lightRowCount;
            _lightsPerRow = lightBlockProperties._arrayCounts.lightsPerRow;

            _primaryColor = lightBlockProperties._lightColors.primaryColor;
            _secondaryColor = lightBlockProperties._lightColors.secondaryColor;
            _tertiaryColor = lightBlockProperties._lightColors.tertiaryColor;
            _colorAssignmentMode = lightBlockProperties._lightColors.colorAssignmentMode;

            _arrangementShape = lightBlockProperties._arrangementData.arrangementShape;
            _arrayOrientation = lightBlockProperties._arrangementData.arrayOrientation;
            _lightOrientation = lightBlockProperties._arrangementData.lightOrientation;

            ArrayTransforms = new Transform[_lightArrayControllerCount];
            LightRowTransforms = new Transform[_lightRowsPerController * _lightArrayControllerCount];
            LightTransforms = new Transform[_lightsPerRow * _lightRowsPerController * _lightArrayControllerCount];
            _selectedLightType = lightBlockProperties._selectedLightObject;
            
            _radius = lightBlockProperties._circularArrangementData.radius;
            _arc = lightBlockProperties._circularArrangementData.arc;
            _coneTightnessFactor = lightBlockProperties._circularArrangementData.coneTightnessFactor;

            _minRotationSpeed = lightBlockProperties._circularArrangementData._minRotationSpeed;
            _arrayRotationSpeed = lightBlockProperties._circularArrangementData._maxRotationSpeed;
            _enableRotation = lightBlockProperties._circularArrangementData.enableArrayRotation;
            _rotationDirection = lightBlockProperties._circularArrangementData.rotationDirection;

            _lightTriggerDirection = lightBlockProperties._lightTriggerDirection;
            _rangeAssignment = lightBlockProperties._spectrumRangeAssignment;

            transform.localPosition = _blockProperties._localPosition;
            transform.localRotation = _blockProperties._blockRotation;
            InitializeLightArray();
        }

        public void InitializeLightArray()
        {
            if (transform.childCount > 0)
            {
                RemoveAll();
            }

            ArrayTransforms = new Transform[_lightArrayControllerCount];
            LightRowTransforms = new Transform[_lightRowsPerController * _lightArrayControllerCount];
            LightTransforms = new Transform[_lightsPerRow * _lightRowsPerController * _lightArrayControllerCount];
            _lightColors = new Color[_lightArrayControllerCount];

            SetColors();
            int lightCounter = 0;
            int lightRowCounter = 0;

            for (int i = 0; i < _lightArrayControllerCount; i++)
            {
                GameObject newLightArrayController = Instantiate(_lightArrayController);
                newLightArrayController.transform.SetParent(transform);
                newLightArrayController.transform.localPosition = new Vector3(0, 0, i * _lightArraySpacing);
                ArrayTransforms[i] = newLightArrayController.transform;

                for (int j = 0; j < _lightRowsPerController; j++)
                {
                    GameObject newLightRow = Instantiate(_lightRowController);
                    newLightRow.transform.SetParent(newLightArrayController.transform);
                    newLightRow.transform.localPosition = new Vector3(0, j * _lightRowSpacing, 0);
                    LightRowTransforms[lightRowCounter] = newLightRow.transform;
                    lightRowCounter++;

                    for (int k = 0; k < _lightsPerRow; k++)
                    {
                        GameObject newLightBlinker = Instantiate(_lightPrefabs[(int)_selectedLightType]);
                        newLightBlinker.transform.SetParent(newLightRow.transform);
                        newLightBlinker.transform.localPosition = new Vector3(0, 0, k * _singleLightSpacing);
                        LightTransforms[lightCounter] = newLightBlinker.transform; 
                        lightCounter++;
                    }
                }
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                LightArrayController currentLightArrayController = transform.GetChild(i).GetComponent<LightArrayController>();
                currentLightArrayController.spectrumRangeAssignment = _rangeAssignment;
                currentLightArrayController.InitializeLightArrayControllerEditor();

                switch (_lightTriggerDirection)
                {
                    case LightArrayDirection.Forward:
                        currentLightArrayController.GetComponent<LightArrayController>().triggerSequence = LightArrayController.TriggerSequence.Forward;
                        break;
                    case LightArrayDirection.Reverse:
                        currentLightArrayController.GetComponent<LightArrayController>().triggerSequence = LightArrayController.TriggerSequence.Reverse;
                        break;
                    case LightArrayDirection.Alternating:
                        currentLightArrayController.GetComponent<LightArrayController>().triggerSequence = LightArrayController.TriggerSequence.Forward;
                        if (i % 2 == 0)
                        {
                            currentLightArrayController.GetComponent<LightArrayController>().triggerSequence = LightArrayController.TriggerSequence.Reverse;
                        }
                        break;
                    case LightArrayDirection.HalfForward:
                        currentLightArrayController.GetComponent<LightArrayController>().triggerSequence = LightArrayController.TriggerSequence.Forward;
                        if (i > transform.childCount / 2)
                        {
                            currentLightArrayController.GetComponent<LightArrayController>().triggerSequence = LightArrayController.TriggerSequence.Reverse;
                        }
                        break;
                    case LightArrayDirection.HalfReverse:
                        currentLightArrayController.GetComponent<LightArrayController>().triggerSequence = LightArrayController.TriggerSequence.Reverse;
                        if (i > transform.childCount / 2)
                        {
                            currentLightArrayController.GetComponent<LightArrayController>().triggerSequence = LightArrayController.TriggerSequence.Forward;
                        }
                        break;
                }
            }

            SetArraySpinDirection();
            ArrangeLights();
        }

        public void SetArraySpinDirection()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                LightArraySpinner currentLightArrayController = transform.GetChild(i).GetComponent<LightArraySpinner>();

                switch (_rotationDirection)
                {
                    case LightArrayDirection.Forward:
                        currentLightArrayController.rotationDirection = LightArrayController.TriggerSequence.Forward;
                        break;
                    case LightArrayDirection.Reverse:
                        currentLightArrayController.rotationDirection = LightArrayController.TriggerSequence.Reverse;
                        break;
                    case LightArrayDirection.Alternating:
                        currentLightArrayController.rotationDirection = LightArrayController.TriggerSequence.Forward;
                        if (i % 2 == 0)
                        {
                            currentLightArrayController.rotationDirection = LightArrayController.TriggerSequence.Reverse;
                        }
                        break;
                    case LightArrayDirection.HalfForward:
                        currentLightArrayController.rotationDirection = LightArrayController.TriggerSequence.Forward;
                        if (i > transform.childCount / 2)
                        {
                            currentLightArrayController.rotationDirection = LightArrayController.TriggerSequence.Reverse;
                        }
                        break;
                    case LightArrayDirection.HalfReverse:
                        currentLightArrayController.rotationDirection = LightArrayController.TriggerSequence.Reverse;
                        if (i > transform.childCount / 2)
                        {
                            currentLightArrayController.rotationDirection = LightArrayController.TriggerSequence.Forward;
                        }
                        break;
                }
            }
        }

        public void SetColors()
        {
            if(_lightColors.Length < transform.childCount)
            {
                _lightColors = new Color[transform.childCount]; 
            }

            int colorBlockSize = _lightColors.Length / 4;
            int colorCounter = 0;
            int lightColorCountFirstHalf = _lightColors.Length / 2;

            switch (_colorAssignmentMode)
            {
                case ColorAssignmentMode.Alternating:
                    for (int i = 0; i < _lightColors.Length; i++)
                    {
                        if (colorCounter <= colorBlockSize / 2)
                        {
                            _lightColors[i] = _primaryColor;
                        }
                        else if (colorCounter > colorBlockSize / 2 && colorCounter <= colorBlockSize * 0.75)
                        {
                            _lightColors[i] = _secondaryColor;
                        }
                        else if (colorCounter > colorBlockSize * 0.75)
                        {
                            _lightColors[i] = _tertiaryColor;
                        }
                        colorCounter = colorCounter == colorBlockSize ? 0 : colorCounter + 1;
                    }
                    break;
                case ColorAssignmentMode.Linear:
                    int colorsCount = _lightColors.Length;
                    for (int i = 0; i < colorsCount; i++)
                    {
                        if (i < colorsCount / 2)
                        {
                            _lightColors[i] = _primaryColor;
                        }
                        else if (i < colorsCount * 0.8)
                        {
                            _lightColors[i] = _secondaryColor;
                        }
                        else if (i < colorsCount)
                        {
                            _lightColors[i] = _tertiaryColor;
                        }
                    }
                    break;
                case ColorAssignmentMode.Symmetrical:
                    for (int i = 0, j = _lightColors.Length - 1; i <= lightColorCountFirstHalf && j > lightColorCountFirstHalf; i++, j--)
                    {
                        if (i < lightColorCountFirstHalf / 2)
                        {
                            _lightColors[i] =
                                _lightColors[j] = _primaryColor;
                        }
                        else if (i < lightColorCountFirstHalf * 0.8)
                        {
                            _lightColors[i] =
                                _lightColors[j] = _secondaryColor;
                        }
                        else if (i <= lightColorCountFirstHalf)
                        {
                            _lightColors[i] =
                                _lightColors[j] = _tertiaryColor;
                        }
                    }
                    break;
                case ColorAssignmentMode.ReverseSymmetrical:
                    for (int i = 0, j = _lightColors.Length - 1; i <= lightColorCountFirstHalf && j > lightColorCountFirstHalf; i++, j--)
                    {
                        if (i < lightColorCountFirstHalf / 2)
                        {
                            _lightColors[i] =
                                _lightColors[j] = _tertiaryColor;
                        }
                        else if (i < lightColorCountFirstHalf * 0.8)
                        {
                            _lightColors[i] =
                                _lightColors[j] = _secondaryColor;
                        }
                        else if (i <= lightColorCountFirstHalf)
                        {
                            _lightColors[i] =
                                _lightColors[j] = _primaryColor;
                        }
                    }
                    break;

                case ColorAssignmentMode.ReverseLinear:
                    for (int i = _lightColors.Length - 1; i > 0; i--)
                    {
                        if (i > _lightColors.Length / 2)
                        {
                            _lightColors[i] = _primaryColor;
                        }
                        else if (i > _lightColors.Length  * 0.2)
                        {
                            _lightColors[i] = _secondaryColor;
                        }
                        else if (i > 0)
                        {
                            _lightColors[i] = _tertiaryColor;
                        }
                    }
                    break;
            }
            SetLightArrayColors();
        }

        public void SetLightArrayColors()
        {
            if (lightArrays == null || lightArrays.Length == 0)
            {
                GetLightArrays();
            }
            for (int i = 0; i < lightArrays.Length; i++)
            {
                //transform.GetChild(i).GetComponent<LightArrayController>().LightColor = _lightColors[i];
                lightArrays[i].LightColor = _lightColors[i];
            }
        }

        public void RemoveAll()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
        #region arrangement methods
        public void ArrangeLights()
        {
            if (ArrayTransforms == null )
            {
                return;
            }
            if(ArrayTransforms.Length == 0)
            {
                return;
            }

            switch (_arrangementShape)
            {
                case LightArrangementShape.Flat:
                    ArrangeLightsFlat();
                    break;
                case LightArrangementShape.Circle:
                    ArrangeLightsCircular();
                    break;
            }
        }
        public void SetArrayRotationSpeed()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                LightArraySpinner currentLightspinner = transform.GetChild(i).GetComponent<LightArraySpinner>();
                currentLightspinner.enabled = _enableRotation;
                currentLightspinner.rotationSpeed = _arrayRotationSpeed;
            }
        }

        private void ArrangeLightsFlat()
        {
            int rowCounter = 0;
            for (int i = 0; i < ArrayTransforms.Length; i++)
            {
                Transform currentArrayController = ArrayTransforms[i];
                Vector3 arrayPosition = new Vector3();

                switch (_arrayOrientation)
                {
                    case LightArrayOrientation.Vertical:
                        arrayPosition.z = (i * _lightArraySpacing) + ((_lightArraySpacing * (_lightArrayControllerCount - 1)) / -2);
                        break;

                    case LightArrayOrientation.Horizontal:
                        arrayPosition.y = (i * _lightArraySpacing);
                        break;
                }
                _enableRotation = false;
                currentArrayController.GetComponent<LightArraySpinner>().enabled = false;
                currentArrayController.transform.localPosition = arrayPosition;
            }


            for (int i = 0; i < LightRowTransforms.Length; i++)
            {
                Transform currentLightRow = LightRowTransforms[i].transform;
                currentLightRow.localPosition = SetLightRowPosition(rowCounter);
                rowCounter = rowCounter == _lightRowsPerController - 1 ? 0 : rowCounter + 1; 
            }
            SetLightPositions(_radius);
        }

        private Vector3 SetLightRowPosition(int j)
        {
            Vector3 lightRowPosition = new Vector3();
            ToggleLightArraySpinner(false);

            switch (_lightOrientation)
            {
                case LightArrayOrientation.Vertical:
                    lightRowPosition.y = _lightRowSpacing * j;
                    lightRowPosition.z = _diagonalOffset * j;
                    if (j % 2 == 0)
                    {
                        lightRowPosition.z += _horizontalOffset;
                    }
                    break;

                case LightArrayOrientation.Horizontal:
                    lightRowPosition.z = _lightRowSpacing * j;
                    lightRowPosition.y = _diagonalOffset * j;
                    if (j % 2 == 0)
                    {
                        lightRowPosition.y += _horizontalOffset;
                    }
                    break;
            }
            lightRowPosition.x = _radius;
            return lightRowPosition;
        }

        private void ToggleLightArraySpinner(bool enable)
        {
            for (int i = 0; i < ArrayTransforms.Length; i++)
            {
                ArrayTransforms[i].gameObject.GetComponent<LightArraySpinner>().enabled = enable;
            }
        }

        private void SetLightPositions(float radius)
        {
            switch (_arrangementShape)
            {
                case LightArrangementShape.Flat:
                    for (int k = 0, lightcounter = 0; lightcounter < LightTransforms.Length; lightcounter++)
                    {
                        Vector3 lightPosition = new Vector3();
                        Quaternion lightRotation = Quaternion.Euler(_lightRotation.x, _lightRotation.y, _lightRotation.z);
                        Quaternion oddLightsRotation = Quaternion.Euler(_oddLightsRotation.x, _oddLightsRotation.y, _oddLightsRotation.z);
                        //lightRotation.x = visualizationManager.lightRotation;

                        switch (_lightOrientation)
                        {
                            case LightArrayOrientation.Vertical:
                                lightPosition.z = _singleLightSpacing * k;
                                if (k % 2 == 0)
                                {
                                    lightPosition.y += _rowVerticalOffset;
                                }
                                break;
                            case LightArrayOrientation.Horizontal:
                                lightPosition.y = _singleLightSpacing * k;
                                if (k % 2 == 0)
                                {
                                    lightPosition.z += _rowVerticalOffset;

                                }
                                break;
                        }

                        Transform currentLight = LightTransforms[lightcounter];

                        //lightCounter = lightCounter >= lightTransforms.Length - 1 ? 0 : lightCounter + 1;
                        //lightCounter++;
                        //if(lightCounter >= lightTransforms.Length)
                        //{
                        //    lightCounter 
                        //}

                        k = k == _lightsPerRow - 1 ? 0 : k + 1;
                        currentLight.localPosition = lightPosition;
                        if (lightcounter % 2 == 0)
                        {
                            currentLight.localRotation = lightRotation;
                        }
                        else
                        {
                            currentLight.localRotation = oddLightsRotation;
                        }
                    }

                    break;
                case LightArrangementShape.Circle:
                    SetLightPositionsCircular(radius);
                    break;
            }
        }
        private void ArrangeLightsCircular()
        {
            float currentRotation = -90;
            int lightRowCounter = 0;

            if (ArrayTransforms == null)
            {
                return;
            }

            if(ArrayTransforms.Length == 0)
            {
                return;
            }

            for (int i = 0; i < ArrayTransforms.Length; i++)
            {
                Transform currentLightArray = ArrayTransforms[i];
                if(currentLightArray == null)
                {
                    return;
                }
                switch (_arrayOrientation)
                {
                    case LightArrayOrientation.Vertical:
                        currentRotation += _arc / _lightArrayControllerCount;
                        currentLightArray.transform.localPosition = Vector3.zero;
                        currentLightArray.GetComponent<LightArraySpinner>().enabled = false;
                        break;

                    case LightArrayOrientation.Horizontal:
                        currentRotation = 0;
                        currentLightArray.GetComponent<LightArraySpinner>().enabled = _enableRotation;
                        currentLightArray.GetComponent<LightArraySpinner>().rotationSpeed = _arrayRotationSpeed;

                        currentLightArray.transform.localPosition = new Vector3(0, i * _lightArraySpacing, 0);
                        break;
                }
                currentLightArray.localRotation = Quaternion.Euler(0, currentRotation, 0);
            }

            for (int j = 0; j < LightRowTransforms.Length; j++)
            {
                Transform currentLightRow = LightRowTransforms[j];
                Vector3 currentLightRowPosition = SetLightRowPosition(lightRowCounter);
                switch (_arrayOrientation)
                {
                    case LightArrayOrientation.Vertical:
                        currentLightRowPosition.x = _radius;
                        currentLightRow.localPosition = currentLightRowPosition;
                        break;
                    case LightArrayOrientation.Horizontal:
                        currentLightRow.localPosition = Vector3.zero;
                        currentLightRow.localRotation = Quaternion.Euler(0, currentRotation += _arc / _lightRowsPerController, 0);
                        break;
                }
                lightRowCounter = lightRowCounter == _lightRowsPerController - 1 ? 0 : lightRowCounter + 1;
            }

            ToggleLightArraySpinner(_arrayOrientation == LightArrayOrientation.Horizontal);

            SetLightPositions(_radius);
        }

        private void SetLightPositionsCircular(float radius)
        {
            int lightRowCounter = 1;
            int lightCounter = 0;
            for (int i = 0; i < LightTransforms.Length; i++)
            {
                Transform currentLight = LightTransforms[i];
                Quaternion lightRotation = Quaternion.Euler(_lightRotation.x, _lightRotation.y, _lightRotation.z);
                Quaternion oddLightsRotation = Quaternion.Euler(_oddLightsRotation.x, _oddLightsRotation.y, _oddLightsRotation.z);

                if (i % _lightRowsPerController == 0 && _coneTightnessFactor > 1)
                {
                    lightRowCounter++;
                }

                currentLight.localPosition = new Vector3(radius + lightRowCounter / _coneTightnessFactor, 0, lightCounter * _singleLightSpacing);
                lightCounter = lightCounter < _lightsPerRow ? lightCounter + 1 : 0;

                if (i % 2 == 0)
                {
                    currentLight.localRotation = lightRotation;
                    continue;
                }
                currentLight.localRotation = oddLightsRotation;
            }
        }
        #endregion

        public void UpdateLightBlockProperties()
        {
            _blockProperties._arrayCounts.lightArrayCount = _lightArrayControllerCount;
            _blockProperties._arrayCounts.lightRowCount = _lightRowsPerController;
            _blockProperties._arrayCounts.lightsPerRow = _lightsPerRow;

            _blockProperties._arrangementData.arrangementShape = _arrangementShape;
            _blockProperties._arrangementData.arrayOrientation = _arrayOrientation;
            _blockProperties._arrangementData.lightOrientation = _lightOrientation;

            _blockProperties._circularArrangementData.arc = _arc;
            _blockProperties._circularArrangementData.coneTightnessFactor = _coneTightnessFactor;
            _blockProperties._circularArrangementData.radius = _radius;
            _blockProperties._circularArrangementData.enableArrayRotation = _enableRotation;
            _blockProperties._circularArrangementData.rotationDirection = _rotationDirection;

            _blockProperties._lightColors.primaryColor = _primaryColor;
            _blockProperties._lightColors.secondaryColor = _secondaryColor;
            _blockProperties._lightColors.tertiaryColor = _tertiaryColor;
            _blockProperties._lightColors.colorAssignmentMode = _colorAssignmentMode;

            _blockProperties._lightTriggerDirection = _lightTriggerDirection;

            _blockProperties._offsets.diagonalOffset = _diagonalOffset;
            _blockProperties._offsets.horizontalOffset = _horizontalOffset;
            _blockProperties._offsets.rowVerticalOffset = _rowVerticalOffset;

            _blockProperties._rotations = new LightRotations(_lightRotation, _oddLightsRotation);

            _blockProperties._selectedLightObject = _selectedLightType;
            _blockProperties._spectrumRangeAssignment = _rangeAssignment;

            _blockProperties._spacingProperties.lightArraySpacing = _lightArraySpacing;
            _blockProperties._spacingProperties.lightRowSpacing = _lightRowSpacing;
            _blockProperties._spacingProperties.singleLightSpacing = _singleLightSpacing;

            _blockProperties._localPosition = transform.localPosition;
        }
        #endregion
    }

    [System.Serializable]
    public struct LightBlockProperties
    {
        public ArraySpacingProperties _spacingProperties;
        public LightArrayCounts _arrayCounts;
        public ArrayOffsets _offsets;
        public LightRotations _rotations;
        public CircularArrangementData _circularArrangementData;
        public LightObject _selectedLightObject;
        public LightColors _lightColors;
        public ArrangementData _arrangementData;
        public LightArrayDirection _lightTriggerDirection;
        public SpectrumRangeAssignment _spectrumRangeAssignment;
        public Vector3 _localPosition;
        public Quaternion _blockRotation;

        public LightBlockProperties(ArraySpacingProperties spacing, LightArrayCounts counts, ArrayOffsets offsets, LightRotations rotations, CircularArrangementData circularArrangementData, LightObject light, LightColors colors, ArrangementData arrangementData, LightArrayDirection lightTriggerDirection, SpectrumRangeAssignment spectrumRange, Vector3 localPosition, Quaternion blockRotation )
        {
            _blockRotation = blockRotation;
            _localPosition = localPosition;
            _spectrumRangeAssignment = spectrumRange;
            _lightTriggerDirection = lightTriggerDirection;
            _spacingProperties = spacing;
            _arrayCounts = counts;
            _offsets = offsets;
            _rotations = rotations;
            _circularArrangementData = circularArrangementData;
            _arrangementData = arrangementData;
            _selectedLightObject = light;
            _lightColors = colors;   
        }
    }

    [System.Serializable]
    public struct ArraySpacingProperties
    {
        public float lightArraySpacing;
        public float lightRowSpacing;
        public float singleLightSpacing;

        public ArraySpacingProperties(float arraySpacing, float rowSpacing, float lightSpacing)
        {
            lightArraySpacing = arraySpacing;
            lightRowSpacing = rowSpacing;
            singleLightSpacing = lightSpacing;
        }
    }

    [System.Serializable]
    public struct ArrangementData 
    {
        public LightArrayOrientation arrayOrientation;
        public LightArrayOrientation lightOrientation;
        public LightArrangementShape arrangementShape;

        public ArrangementData(LightArrayOrientation arrayOrientation, LightArrayOrientation lightOrientation, LightArrangementShape arrangementShape)
        {
            this.arrayOrientation = arrayOrientation;
            this.lightOrientation = lightOrientation;
            this.arrangementShape = arrangementShape;
        }
    }

    [System.Serializable]
    public struct LightArrayCounts
    {
        public int lightArrayCount;
        public int lightRowCount;
        public int lightsPerRow;

        public LightArrayCounts(int arrayCount, int rowCount, int lightsCount)
        {
            lightArrayCount = arrayCount;
            lightRowCount = rowCount;
            lightsPerRow = lightsCount;
        }
    }

    [System.Serializable]
    public struct ArrayOffsets
    {
        public float diagonalOffset;
        public float horizontalOffset;
        public float rowVerticalOffset;

        public ArrayOffsets(float diagonal, float horizontal, float rowVertical)
        {
            diagonalOffset = diagonal;
            horizontalOffset = horizontal;
            rowVerticalOffset = rowVertical;
        }
    }

    [System.Serializable]
    public struct LightRotations
    {
        public Vector3 lightRotation;
        public Vector3 oddLightsRotation;

        public LightRotations(Vector3 evenLightsRotxn, Vector3 oddLightsRotxn)
        {
            lightRotation = evenLightsRotxn;
            oddLightsRotation = oddLightsRotxn;
        }
    }

    [System.Serializable]
    public struct LightColors
    {
        public Color primaryColor;
        public Color secondaryColor;
        public Color tertiaryColor;
        public ColorAssignmentMode colorAssignmentMode;
        public LightColors(Color primaryColor, Color secondaryColor, Color tertiaryColor, ColorAssignmentMode colorAssignment)
        {
            this.primaryColor = primaryColor;
            this.secondaryColor = secondaryColor;
            this.tertiaryColor = tertiaryColor;
            colorAssignmentMode = colorAssignment;
        }
    }

    [System.Serializable]
    public struct CircularArrangementData
    {
        public float arc;
        public float radius;
        public float coneTightnessFactor;
        public bool enableArrayRotation;
        public float _minRotationSpeed;
        public float _maxRotationSpeed;
        public LightArrayDirection rotationDirection;

        public CircularArrangementData(float radius, float coneTightnessFactor, float arc, bool enableRotation, LightArrayDirection rotationDirection, float minRotationSpeed, float maxRotationSpeed)
        {
            _minRotationSpeed = minRotationSpeed;
            _maxRotationSpeed = maxRotationSpeed;
            enableArrayRotation = enableRotation;
            this.rotationDirection = rotationDirection;
            this.radius = radius;
            this.arc = arc;
            this.coneTightnessFactor = coneTightnessFactor;
        }
    }

    public enum LightArrayOrientation
    {
        Vertical,
        Horizontal
    }

    public enum EQBarSpectrumAssignment
    {
        CenterPeak,
        SidePeak,
        Linear,
        StaggeredPeaks
    }

    public enum RangeAssignmentMode
    {
        MiddleTopRange,
        SideTopRange,
        Linear
    }


    public enum ColorAssignmentMode
    {
        Alternating,
        Linear,
        Symmetrical,
        ReverseSymmetrical,
        ReverseLinear
    }

    public enum LightArrangementShape
    {
        Flat,
        Circle,
    }

    public enum LightObject
    {
        Cube = 0,
        Rectangle = 1,
        Pyramid =2,
        Sphere = 3,
        SpotLight = 4

    }

    public enum LightArrayDirection
    {
        Forward,
        Reverse,
        Alternating,
        HalfForward,
        HalfReverse
    }
}
