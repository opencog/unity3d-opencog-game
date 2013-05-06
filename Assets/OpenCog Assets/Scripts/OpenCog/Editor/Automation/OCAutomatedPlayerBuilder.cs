
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
using UnityEngine;
using System.Collections;
using ProtoBuf;

namespace OpenCog
{

namespace Automation
{

/// <summary>
/// The OpenCog OCAutomatedPlayerBuilder.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[ExecuteInEditMode]
#endregion
public class OCAutomatedPlayerBuilder : MonoBehaviour
{

  /////////////////////////////////////////////////////////////////////////////

  #region Private Member Data

  /////////////////////////////////////////////////////////////////////////////

  private int _examplePrivateVar = 0;

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Accessors and Mutators

  /////////////////////////////////////////////////////////////////////////////

  public int ExamplePublicVar
  {
    get
    {
      return _examplePrivateVar;
    }

    set
    {
      _examplePrivateVar = value;
    }
  }

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Public Member Functions

  /////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Called when the script instance is being loaded.
  /// </summary>
  void Awake()
  {
  }

  /// <summary>
  /// Use this for initialization
  /// </summary>
  void Start()
  {
  }

  /// <summary>
  /// Update is called once per frame.
  /// </summary>
  void Update()
  {
  }

  /// <summary>
  /// Called once per frame after all Update calls
  /// </summary>
  void LateUpdate()
  {
  }

  /// <summary>
  /// Raises the enable event when OCAutomatedPlayerBuilder is loaded.
  /// </summary>
  void OnEnable()
  {
    Debug.Log
    (
      string.Format
      (
        "MonoBehaviour[{0}].OnEnable"
      , gameObject.name + "\\" + GetType().Name
      )
    );
  }

  /// <summary>
  /// Raises the disable event when OCAutomatedPlayerBuilder goes out of scope.
  /// </summary>
  void OnDisable()
  {
    Debug.Log
    (
      string.Format
      (
        "MonoBehaviour[{0}].OnDisable"
      , gameObject.name + "\\" + GetType().Name
      )
    );
  }

  /// <summary>
  /// Raises the destroy event when OCAutomatedPlayerBuilder is about to be destroyed.
  /// </summary>
  void OnDestroy()
  {
    Debug.Log
    (
      string.Format
      (
        "MonoBehaviour[{0}].OnDestroy"
      , gameObject.name + "\\" + GetType().Name
      )
    );
  }

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

  #region Private Member Functions

  /////////////////////////////////////////////////////////////////////////////

  /////////////////////////////////////////////////////////////////////////////

  #endregion

  /////////////////////////////////////////////////////////////////////////////

}// class OCAutomatedPlayerBuilder

}// namespace Automation

}// namespace OpenCog



