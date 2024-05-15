using System.Collections.Generic;
using UnityEngine;

using Fusion;
namespace FusionHelpers
{
	/// <summary>
	/// Example of a Fusion Object Pool.
	/// The pool keeps a list of available instances by prefab and also a list of which pool each instance belongs to.
	/// </summary>

	public class FusionObjectPoolRoot : MonoBehaviour, INetworkObjectProvider
	{
		private Dictionary<object, FusionObjectPool> _poolsByPrefab = new Dictionary<object, FusionObjectPool>();
		private Dictionary<NetworkObject, FusionObjectPool> _poolsByInstance = new Dictionary<NetworkObject, FusionObjectPool>();


		public FusionObjectPool GetPool<T>(T prefab) where T : NetworkObject
		{
			FusionObjectPool pool;
			if (!_poolsByPrefab.TryGetValue(prefab, out pool))
			{
				pool = new FusionObjectPool();
				_poolsByPrefab[prefab] = pool;
			}

			return pool;
		}
		public NetworkObjectAcquireResult AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context, out NetworkObject result)
        {
            NetworkObject prefab;

			if (NetworkProjectConfig.Global.PrefabTable.Contains(context.PrefabId))
			{
				prefab = NetworkProjectConfig.Global.PrefabTable.Load(context.PrefabId, true);

				FusionObjectPool pool = GetPool(prefab);
				NetworkObject newt = pool.GetFromPool(Vector3.zero, Quaternion.identity);

				if (newt == null)
				{
					Debug.Log($"Creating new instance for prefab {prefab}");
					newt = Instantiate(prefab, Vector3.zero, Quaternion.identity);
					_poolsByInstance[newt] = pool;

					newt.gameObject.SetActive(true);
                    result= newt;

                    return NetworkObjectAcquireResult.Success;
                }
			}

            result = null;
            return NetworkObjectAcquireResult.Failed;

        }

        public void ReleaseInstance(NetworkRunner runner, in NetworkObjectReleaseContext context)
		{
			var no = context.Object;
			
			Debug.Log($"Releasing {no} instance, isSceneObject={context.TypeId.IsSceneObject}");
			if (no != null)
			{
				FusionObjectPool pool;
				if (_poolsByInstance.TryGetValue(no, out pool))
				{
					pool.ReturnToPool(no);
					no.gameObject.SetActive(false); // Should always disable before re-parenting, or we will dirty it twice
					no.transform.SetParent(transform, false);
				}
				else
				{
					no.gameObject.SetActive(false); // Should always disable before re-parenting, or we will dirty it twice
					no.transform.SetParent(null, false);
					Destroy(no.gameObject);
				}
			}
		}

		public void ClearPools()
		{
			foreach (FusionObjectPool pool in _poolsByPrefab.Values)
			{
				pool.Clear();
			}

			foreach (FusionObjectPool pool in _poolsByInstance.Values)
			{
				pool.Clear();
			}

			_poolsByPrefab = new Dictionary<object, FusionObjectPool>();
		}
	}
}