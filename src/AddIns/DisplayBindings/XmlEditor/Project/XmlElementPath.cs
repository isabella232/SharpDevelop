//
// SharpDevelop Xml Editor
//
// Copyright (C) 2005 Matthew Ward
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Matthew Ward (mrward@users.sourceforge.net)


using System;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Represents the path to an xml element starting from the root of the
	/// document.
	/// </summary>
	public class XmlElementPath
	{
		QualifiedNameCollection elements = new QualifiedNameCollection();
		
		public XmlElementPath()
		{
		}
		
		/// <summary>
		/// Gets the elements specifying the path.
		/// </summary>
		/// <remarks>The order of the elements determines the path.</remarks>
		public QualifiedNameCollection Elements {
			get {
				return elements;
			}
		}
		
		/// <summary>
		/// Compacts the path so it only contains the elements that are from 
		/// the namespace of the last element in the path. 
		/// </summary>
		/// <remarks>This method is used when we need to know the path for a
		/// particular namespace and do not care about the complete path.
		/// </remarks>
		public void Compact()
		{
			if (elements.Count > 0) {
				QualifiedName lastName = Elements[Elements.Count - 1];
				if (lastName != null) {
					int index = FindNonMatchingParentElement(lastName.Namespace);
					if (index != -1) {
						RemoveParentElements(index);
					}
				}
			}
		}
		
		/// <summary>
		/// An xml element path is considered to be equal if 
		/// each path item has the same name and namespace.
		/// </summary>
		public override bool Equals(object obj) {
			
			if (!(obj is XmlElementPath)) return false;
			if (this == obj) return true;
			
			XmlElementPath rhs = (XmlElementPath)obj;
			if (elements.Count == rhs.elements.Count) {
				
				for (int i = 0; i < elements.Count; ++i) {
					if (!elements[i].Equals(rhs.elements[i])) {
						return false;
					}
				}
				return true;
			}
			
			return false;
		}
		
		public override int GetHashCode() {
			return elements.GetHashCode();
		}
		
		/// <summary>
		/// Removes elements up to and including the specified index.
		/// </summary>
		void RemoveParentElements(int index)
		{
			while (index >= 0) {
				--index;
				elements.RemoveFirst();
			}
		}
		
		/// <summary>
		/// Finds the first parent that does belong in the specified
		/// namespace.
		/// </summary>
		int FindNonMatchingParentElement(string namespaceUri)
		{
			int index = -1;
			
			if (elements.Count > 1) {
				// Start the check from the the last but one item.
				for (int i = elements.Count - 2; i >= 0; --i) {
					QualifiedName name = elements[i];
					if (name.Namespace != namespaceUri) {
						index = i;
						break;
					}
				}
			}
			return index;
		}
	}
}
