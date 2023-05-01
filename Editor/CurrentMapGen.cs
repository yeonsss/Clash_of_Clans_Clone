using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEditor.SceneManagement;
using static Define;

namespace Editor
{
    public class CurrentMapGen : EditorWindow
    {
        private GameObject _board;
        private GameObject _buildings;
        private CommandStack _cStack = new CommandStack();
        
        private Vector2 _scrollPos = Vector2.zero;
        private float _worldPosY;
        private float _worldPosX;
        private Vector3 _prevPos;

        private GUIStyle _blockStyle;
        private GUIStyle _itemStyle;
        private const string C_PrefabsPath = "Prefabs/Building";
        private string _mapGenPath;

        private readonly Vector3 _spawnPos = new Vector3(10, 0, -10);
        private readonly Vector3 _spawnPosDiv2 = new Vector3(10.5f, 0, -10.5f);
        
        private Dictionary<int, Build> _prevObjDict;
        private static GameObject[,] _map = new GameObject[50, 50];
        
        private WindowStyle _style;
        
        public enum BuildingSize
        {
            DefenceTower = 2,
            GoldMine = 3,
            Hall = 5,
            Wall = 1,
        }

        private Dictionary<BuildName, string[]> _levelDict = new Dictionary<BuildName, string[]>()
        {
            {
                BuildName.Hall,
                new[] { "Level 1", "Level 2", "Level 3", "Level4" }
            },
            {
                BuildName.DefenceTower,
                new[] { "Level 1", "Level 2", "Level 3", "Level4" }
            },
            {
                BuildName.GoldMine,
                new[] { "Level 1", "Level 2", "Level 3", "Level4" }
            },
            {
                BuildName.Wall,
                new[] { "Level 1", "Level 2", "Level 3", "Level4" }
            },
        };

        public float WorldPosY
        {
            get
            {
                return _worldPosY;
            }
            set
            {
                _worldPosY = -1 * (value - (Define.HEIGHT / 2));
            }
        }
        
        public float WorldPosX
        {
            get
            {
                return _worldPosX;
            }
            set
            {
                _worldPosX = value - (Define.WIDTH / 2);
            }
        }

        private void Awake()
        {
            _board = GameObject.Find("Board");
            if (_board == null) _board = new GameObject { name = "Board" };
            _buildings = GameObject.Find("Buildings");
            if (_buildings == null) _buildings = new GameObject { name = "Buildings" };
        }

        [MenuItem("Tools/SinglePlay Map Generate")]
        public static void ShowWindow()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.name == "SinglePlayEditor")
            {
                GetWindow<CurrentMapGen>("SinglePlay Map Generate");
            }
            else
            {
                Debug.LogError("Scene Check!!");
            }
        }

        private void OnEnable()
        {
            minSize = new Vector2(400, 600);
            _prevObjDict = new Dictionary<int, Build>();
            _style = new WindowStyle();
            _mapGenPath = "";
            UpdatePrevObjDict();

            SetSnapMovement();

            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorSceneManager.sceneSaving -= OnSceneSaving;
            EditorSceneManager.sceneSaving += OnSceneSaving;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        private void OnDestroy()
        {
            Vector3 snapValue = new Vector3(0.25f, 0.25f, 0.25f);
            EditorSnapSettings.move = snapValue;
            SceneView.duringSceneGui -= OnSceneGUI;
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorSceneManager.sceneSaving -= OnSceneSaving;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
        }

        private void SetSnapMovement()
        {
            Vector3 snapValue = new Vector3(1.0f, 1.0f, 1.0f);
            EditorSnapSettings.move = snapValue;
        }
        
        private void OnHierarchyChanged()
        {
            var currentBuilds = _buildings.transform.GetComponentsInChildren<Build>();
            var currentBuildDict = new Dictionary<int, Build>();

            foreach (var build in currentBuilds)
            {
                var instanceId = build.GetInstanceID();
                currentBuildDict[instanceId] = build;
            }

            _prevObjDict = currentBuildDict;
        }
        
        private void OnSceneSaving(UnityEngine.SceneManagement.Scene scene, string path)
        {
            UpdatePrevObjDict();
        }

        private void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
        {
            UpdatePrevObjDict();
        }

        private void UpdatePrevObjDict()
        {
            var currentBuilds = _buildings.transform.GetComponentsInChildren<Build>();

            foreach (var build in currentBuilds)
            {
                var instanceId = build.GetInstanceID();
                _prevObjDict[instanceId] = build;
            }
        }

        private void Update()
        {
            if (_buildings == null) return;
            
            foreach (var build in _buildings.GetComponentsInChildren<Build>())
            {
                var area = build.transform.GetChild(0);
                if (area == null) continue;
                
                BuildAreaCheck(area);
            }
        }

        private void BuildAreaCheck(Transform area)
        {
            foreach (var range in area.GetComponentsInChildren<BuildArea>())
            {
                range.CollisionCheckForEditMode();
            }
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            
            // if (Event.current != null && Event.current.type == EventType.MouseDown)
            // {
            //     var obj = Selection.activeGameObject;
            //     if (obj == null) return;
            //     
            //     _prevPos = obj.transform.position;
            // }
            // if (Event.current != null && Event.current.type == EventType.MouseUp)
            // {
            //     var obj = Selection.activeGameObject;
            //     if (obj == null) return;
            //     
            //     bool isSamePos = (Vector3.Distance(_prevPos, obj.transform.position) <= 0.01f);
            //
            //     if (isSamePos == false)
            //     {
            //         Debug.Log("none same pos");
            //         var pos = obj.transform.position;
            //         _cStack.Push(new Command()
            //         {
            //             instanceId = obj.GetInstanceID(),
            //             posX = pos.x,
            //             posY = pos.z,
            //         });
            //     }
            // }
        }

        private void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            EditorGUILayout.BeginVertical(_style.GetStyle(Styles.Block));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Board", GUILayout.Height(30)))
            {
                SpawnBoard();
            }
            if (GUILayout.Button("Revert Position", GUILayout.Height(30)))
            {
                // var cm = _cStack.Undo();
                // if (cm == null) return;
                //
                // GameObject foundObject = EditorUtility.InstanceIDToObject(cm.instanceId) as GameObject;
                // if (foundObject == null) return;
                //
                // foundObject.transform.position = new Vector3(cm.posX, 0, cm.posY);
            }
            if (GUILayout.Button("Delete All Building", GUILayout.Height(30)))
            {
                DeleteAllBuild();
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            EditorGUILayout.BeginVertical(_style.GetStyle(Styles.Block));
            ShowBuildingSpawnButton();
            EditorGUILayout.EndVertical();
            GUILayout.Space(10);

            EditorGUILayout.BeginVertical(_style.GetStyle(Styles.Block));
            GUILayout.Label("SaveFile Name", _style.GetStyle(Styles.Lable));
            _mapGenPath = GUILayout.TextField(_mapGenPath, 100);
            if (GUILayout.Button("MapGen", GUILayout.Height(40)))
            {
                // Json 파일로 생성
                MapGen();
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndScrollView();
        }
        
        private void MapGen()
        {
            if (string.IsNullOrEmpty(_mapGenPath)) return;
            
            var filePath = Path.Combine(Application.dataPath, $"{_mapGenPath}.json");
            Debug.Log(filePath);

            List<SinglePlayBuildInfo> buildInfos = new List<SinglePlayBuildInfo>();

            foreach (var build in _buildings.GetComponentsInChildren<Build>())
            {
                var pos = build.transform.position;
                buildInfos.Add(new SinglePlayBuildInfo()
                {
                    posX = (int)pos.x,
                    posY = (int)pos.z,
                    level = build.currentLevel,
                    buildName = build.name,
                });
            }

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                string jsonString = JsonConvert.SerializeObject(buildInfos);
                writer.Write(jsonString);
            }
        }

        private void ShowBuildingSpawnButton()
        {
            var prefabs = Resources.LoadAll<GameObject>(C_PrefabsPath);
            foreach (var prefab in prefabs)
            {
                if (prefab.name == "SurroundWall") continue;
                
                Texture2D icon = AssetPreview.GetAssetPreview(prefab);
                // 레벨 선택 창이 있었으면 좋겠다.
                GUIContent content = new GUIContent() {
                    text = $"{prefab.name}",
                    image = icon,
                };

                EditorGUILayout.BeginHorizontal(_style.GetStyle(Styles.BuildItem));

                EditorGUILayout.BeginVertical();
                GUILayout.Box(content, new GUIStyle()
                {
                    imagePosition = ImagePosition.ImageAbove
                });
                // GUILayout.Label(icon);
                // GUILayout.Label(prefab.name, _style.GetStyle(Styles.Lable));
                EditorGUILayout.EndVertical();
                
                
                EditorGUILayout.BeginVertical();
                if (Enum.TryParse(prefab.name, out BuildingSize size))
                {
                    int s = (int)size;
                    GUILayout.Label($"Size : {s} x {s} ", _style.GetStyle(Styles.Lable));    
                }
                
                if (Enum.TryParse<BuildName>(prefab.name, out var buildName))
                {
                    EditorGUILayout.BeginVertical(_style.GetStyle(Styles.Popup));
                    GUILayout.Label("Select Level", _style.GetStyle(Styles.PopupLable));
                    var selectLevel = EditorGUILayout.Popup(0, _levelDict[buildName]);
                    EditorGUILayout.EndVertical();
                    
                    if (GUILayout.Button("Spawn"))
                    {
                        SpawnBuilding(prefab, selectLevel+1);
                    }
                }
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.EndHorizontal();
            }
        }

        private void SpawnBuilding(GameObject prefab, int level)
        {
            if (Enum.TryParse(prefab.name, out BuildingSize size))
            {
                var obj = (int)size % 2 == 0
                    ? Instantiate(prefab, _spawnPosDiv2, Quaternion.identity, _buildings.transform)
                    : Instantiate(prefab, _spawnPos, Quaternion.identity, _buildings.transform);


                var b = obj.GetComponent<Build>();
                b.XSize = (int)size;
                b.YSize = (int)size;
                b.currentLevel = level;
                b.CulcArea();

                var area = obj.transform.Find("Area");
                area.gameObject.SetActive(true);

                foreach (var bArea in area.GetComponentsInChildren<BuildArea>())
                {
                    bArea.Awake();
                    bArea.EditModeInit();
                }
                
                // var pos = obj.transform.position;
                // _cStack.Push(new Command()
                // {
                //     instanceId = obj.GetInstanceID(),
                //     posX = pos.x,
                //     posY = pos.z,
                // });
            }
        }

        private void DeleteAllBuild()
        {
            var buildCount = _buildings.transform.childCount;
            Debug.Log(buildCount);
            if (buildCount < 1) return;

            for (int i = 0; i < buildCount; i++)
            {
                DestroyImmediate(_buildings.transform.GetChild(0).gameObject);
            }
        }

        private void SpawnBoard()
        {
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/Plane");
            
            for (int i = 0; i < WIDTH; i++)
            {
                WorldPosY = (float)i;
                for(int j = 0; j < HEIGHT; j++)
                {
                    WorldPosX = (float)j;
                    Rtow(j, i, (int)WIDTH, out var wx, out var wy);
                    GameObject obj = Instantiate(prefab, new Vector3(wx, -0.5f, wy), Quaternion.identity, _board.transform);
                    if ((i + j) % 2 == 1)
                    {
                        obj.GetComponent<Area>().EmptyType = 1;
                    }
                    else
                    {
                        obj.GetComponent<Area>().EmptyType = 0;
                    }
                    obj.name = $"{-1 * (i - (HEIGHT / 2))}-{j - (WIDTH / 2)}";
                    
                    _map[i, j] = obj;
                }
                
            }
        }
    }
}


