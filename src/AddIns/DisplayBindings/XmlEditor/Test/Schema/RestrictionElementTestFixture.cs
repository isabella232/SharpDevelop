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

using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.IO;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Tests complex content restriction elements.
	/// </summary>
	[TestFixture]
	public class RestrictionElementTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		ICompletionData[] childElements;
		ICompletionData[] attributes;
		ICompletionData[] annotationChildElements;
		ICompletionData[] choiceChildElements;
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			StringReader reader = new StringReader(GetSchema());
			schemaCompletionData = new XmlSchemaCompletionData(reader);
			
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("group", "http://www.w3.org/2001/XMLSchema"));
			childElements = schemaCompletionData.GetChildElementCompletionData(path);
			attributes = schemaCompletionData.GetAttributeCompletionData(path);
		
			// Get annotation child elements.
			path.Elements.Add(new QualifiedName("annotation", "http://www.w3.org/2001/XMLSchema"));
			annotationChildElements = schemaCompletionData.GetChildElementCompletionData(path);
			
			// Get choice child elements.
			path.Elements.RemoveLast();
			path.Elements.Add(new QualifiedName("choice", "http://www.w3.org/2001/XMLSchema"));
			choiceChildElements = schemaCompletionData.GetChildElementCompletionData(path);
		}

		[Test]
		public void GroupChildElementIsAnnotation()
		{
			Assert.IsTrue(base.Contains(childElements, "annotation"), 
			              "Should have a child element called annotation.");
		}
		
		[Test]
		public void GroupChildElementIsChoice()
		{
			Assert.IsTrue(base.Contains(childElements, "choice"), 
			              "Should have a child element called choice.");
		}		
		
		[Test]
		public void GroupChildElementIsSequence()
		{
			Assert.IsTrue(base.Contains(childElements, "sequence"), 
			              "Should have a child element called sequence.");
		}		
		
		[Test]
		public void GroupAttributeIsName()
		{
			Assert.IsTrue(base.Contains(attributes, "name"),
			              "Should have an attribute called name.");			
		}
		
		[Test]
		public void AnnotationChildElementIsAppInfo()
		{
			Assert.IsTrue(base.Contains(annotationChildElements, "appinfo"), 
			              "Should have a child element called appinfo.");
		}	
		
		[Test]
		public void AnnotationChildElementIsDocumentation()
		{
			Assert.IsTrue(base.Contains(annotationChildElements, "documentation"), 
			              "Should have a child element called appinfo.");
		}	
		
		[Test]
		public void ChoiceChildElementIsSequence()
		{
			Assert.IsTrue(base.Contains(choiceChildElements, "element"), 
			              "Should have a child element called element.");
		}	
		
		string GetSchema()
		{
			return "<xs:schema targetNamespace=\"http://www.w3.org/2001/XMLSchema\" blockDefault=\"#all\" elementFormDefault=\"qualified\" version=\"1.0\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xml:lang=\"EN\" xmlns:hfp=\"http://www.w3.org/2001/XMLSchema-hasFacetAndProperty\">\r\n" +
					"\r\n" +
					" <xs:element name=\"group\" type=\"xs:namedGroup\" id=\"group\">\r\n" +
					" </xs:element>\r\n" +
					"\r\n" +
					" <xs:element name=\"annotation\" id=\"annotation\">\r\n" +
					"   <xs:complexType>\r\n" +
					"      <xs:choice minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n" +
					"       <xs:element name=\"appinfo\"/>\r\n" +
					"       <xs:element name=\"documentation\"/>\r\n" +
					"      </xs:choice>\r\n" +
					"      <xs:attribute name=\"id\" type=\"xs:ID\"/>\r\n" +
					"   </xs:complexType>\r\n" +
					" </xs:element>\r\n" +
					"\r\n" +
					"\r\n" +
					" <xs:complexType name=\"namedGroup\">\r\n" +
					"  <xs:complexContent>\r\n" +
					"   <xs:restriction base=\"xs:realGroup\">\r\n" +
					"    <xs:sequence>\r\n" +
					"     <xs:element ref=\"xs:annotation\" minOccurs=\"0\"/>\r\n" +
					"     <xs:choice minOccurs=\"1\" maxOccurs=\"1\">\r\n" +
					"      <xs:element ref=\"xs:choice\"/>\r\n" +
					"      <xs:element name=\"sequence\"/>\r\n" +
					"     </xs:choice>\r\n" +
					"    </xs:sequence>\r\n" +
					"    <xs:attribute name=\"name\" use=\"required\" type=\"xs:NCName\"/>\r\n" +
					"    <xs:attribute name=\"ref\" use=\"prohibited\"/>\r\n" +
					"    <xs:attribute name=\"minOccurs\" use=\"prohibited\"/>\r\n" +
					"    <xs:attribute name=\"maxOccurs\" use=\"prohibited\"/>\r\n" +
					"    <xs:anyAttribute namespace=\"##other\" processContents=\"lax\"/>\r\n" +
					"   </xs:restriction>\r\n" +
					"  </xs:complexContent>\r\n" +
					" </xs:complexType>\r\n" +
					"\r\n" +
					" <xs:complexType name=\"realGroup\">\r\n" +
					"    <xs:sequence>\r\n" +
					"     <xs:element ref=\"xs:annotation\" minOccurs=\"0\"/>\r\n" +
					"     <xs:choice minOccurs=\"0\" maxOccurs=\"1\">\r\n" +
					"      <xs:element name=\"all\"/>\r\n" +
					"      <xs:element ref=\"xs:choice\"/>\r\n" +
					"      <xs:element name=\"sequence\"/>\r\n" +
					"     </xs:choice>\r\n" +
					"    </xs:sequence>\r\n" +
					"    <xs:anyAttribute namespace=\"##other\" processContents=\"lax\"/>\r\n" +
					" </xs:complexType>\r\n" +
					"\r\n" +
					" <xs:element name=\"choice\" id=\"choice\">\r\n" +
					"   <xs:complexType>\r\n" +
					"     <xs:choice minOccurs=\"0\" maxOccurs=\"1\">\r\n" +
					"       <xs:element name=\"element\"/>\r\n" +
				    "       <xs:element name=\"sequence\"/>\r\n" +
					"     </xs:choice>\r\n" +					
					"   </xs:complexType>\r\n" +
					" </xs:element>\r\n" +
					"</xs:schema>";
		}
	}
}
