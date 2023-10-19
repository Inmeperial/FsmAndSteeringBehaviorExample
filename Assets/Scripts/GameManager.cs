using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(FlyweightManager))]
public class GameManager : MonoBehaviour
{
    public ParticleSystem allHealParticle;
    public ParticleSystem allBlockParticle;
    public ParticleSystem focusToMinionParticle;
    public ParticleSystem randomParticle;

    public LayerMask layersInSightForAgent;
    public GameObject herosAndMinions;

    private FlyweightManager _flyweightManager;
    private RayCastForNodes _raycastForNodes;
    private Agent _agent;
    private float _radiusScanNode = 3;
    private bool _isRainStarted;

    public static GameManager gameManagerStatic;

    void Awake()
    {
        gameManagerStatic = this;
        _agent = this.gameObject.GetComponent<Agent>();
        _agent.LayersInSight = layersInSightForAgent;
        _raycastForNodes = this.gameObject.GetComponent<RayCastForNodes>();
        FlyweightManager = this.gameObject.GetComponent<FlyweightManager>();
    }

    void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && _isRainStarted != true)
        {
            _raycastForNodes.StartRainAndSetNeightbourds();
            _isRainStarted = true;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && herosAndMinions != null)
        {
            herosAndMinions.SetActive(true);
        }
    }


    public FlyweightManager FlyweightManager
    {
        get { return _flyweightManager; }
        private set { _flyweightManager = value; }
    }

    public List<Node> GetPathTheta(Node init, Node end)
    {
        return _agent.GetPathFindingTheta(init, end);
    }

    public Node GetNodeCloserToObj(GameObject obj)
    {
        Collider[] colliders = Physics.OverlapSphere(obj.transform.position, _radiusScanNode);
        Collider nearestCollider = null;
        float minSqrDistance = Mathf.Infinity;
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<Node>() != null)
            {
                float sqrDistanceToCenter = (obj.transform.position - colliders[i].transform.position).sqrMagnitude;
                if (sqrDistanceToCenter < minSqrDistance)
                {
                    minSqrDistance = sqrDistanceToCenter;
                    nearestCollider = colliders[i];
                }
            }
        }

        if (nearestCollider == null)
        {
            Debug.Log("Null return GetNodeCloserToObj.");
        }

        return nearestCollider.gameObject.GetComponent<Node>();
    }

    public List<GameObject> GetListOfNodes()
    {
        return _raycastForNodes.ListOfObjNodes;
    }

    public void CreateParticle(GameObject obj, StateHeroEnum heroEnum)
    {
        Vector3 pos = new Vector3(obj.gameObject.transform.position.x, obj.gameObject.transform.position.y + 1, obj.gameObject.transform.position.z);
        ParticleSystem particle = heroEnum switch
        {
            StateHeroEnum.hyperHeal => allHealParticle,
            StateHeroEnum.allBlocking => allBlockParticle,
            StateHeroEnum.focusToMinion => focusToMinionParticle,
            StateHeroEnum.random => randomParticle,
            _ => allHealParticle,
        };
        Instantiate(particle, pos, Quaternion.identity);

    }

}
