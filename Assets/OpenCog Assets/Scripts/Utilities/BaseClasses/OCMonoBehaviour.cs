/// Unity3D OpenCog World Embodiment Program
/// Copyright (C) 2013  Novamente            
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as
/// published by the Free Software Foundation, either version 3 of the
/// License, or (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.Collections.Generic;
using OpenCog.Attributes;
using ProtoBuf;
using UnityEngine;
using OCID = System.Guid;

namespace OpenCog.Extensions
{

	/// <summary>
	/// The OpenCog MonoBehaviour base class.
	/// </summary>
	#region Class Attributes
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	[OCExposePropertyFields]
	[Serializable]
	#endregion

	public class OCMonoBehaviour : MonoBehaviour
	{
		private OCID _ID;

		public OCID ID 
		{
			get {return _ID;}
		}		

		//---------------------------------------------------------------------------

		#region Public Member Functions

		//---------------------------------------------------------------------------

		//Example Safe Invoke
		//@TODO: Write Safe StartCoroutine and Safe Instantiate

		/// <summary>
		/// Invoke the specified task with parameter time.
		/// </summary>
		/// <param name='task'>
		/// The delegate Task.
		/// </param>
		/// <param name='time'>
		/// The time parameter to pass to the task.
		/// </param>
		public void Invoke(Task task, float time)
		{
			Invoke(task.Method.Name, time);
		}

		/// <summary>
		/// Gets the first component that implements the specified interface.
		/// </summary>
		/// <returns>
		/// The first component that implements the specified interface.
		/// </returns>
		/// <typeparam name='I'>
		/// The type of interface implemented by the component.
		/// </typeparam>
		public I GetInterfaceComponent<I>() where I : class
		{
			return GetComponent(typeof(I)) as I;
		}

		/// <summary>
		/// Finds a list of objects that implement the specified interface.
		/// </summary>
		/// <returns>
		/// A list of objects that implement the specified interface.
		/// </returns>
		/// <typeparam name='I'>
		/// The type of interface implemented by the objects in the list.
		/// </typeparam>
		public static List<I> FindObjectsOfInterface<I>() where I : class
		{
			MonoBehaviour[] monoBehaviours = (MonoBehaviour[])UnityEngine.Object.FindObjectsOfType(typeof(MonoBehaviour));
			List<I> list = new List<I>();
		
			foreach(MonoBehaviour behaviour in monoBehaviours)
			{
				I component = behaviour.GetComponent(typeof(I)) as I;
		 
				if(component != null)
				{
					list.Add(component);
				}
			}
		 
			return list;
		}

		//---------------------------------------------------------------------------

		#endregion

		//---------------------------------------------------------------------------

		#region Other Members

		//---------------------------------------------------------------------------        

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenCog.OCMonoBehaviour"/>
		/// class.  Generally, intitialization should occur in the Start function.
		/// </summary>
		public OCMonoBehaviour()
		{
			_ID = Guid.NewGuid();
		}

		public delegate void Task(float time);

		//---------------------------------------------------------------------------

		#endregion

		//---------------------------------------------------------------------------

	}// class OCMonoBehaviour

}// namespace OpenCog.Extensions




