using System;
using System.IO;
using System.Xml;
using Server.Logging;

namespace Server.Engines.Reports
{
	public abstract class PersistanceReader
	{
		public abstract int GetInt32( string key );
		public abstract bool GetBoolean( string key );
		public abstract string GetString( string key );
		public abstract DateTime GetDateTime( string key );

		public abstract bool BeginChildren();
		public abstract void FinishChildren();
		public abstract bool HasChild{ get; }
		public abstract PersistableObject GetChild();

		public abstract void ReadDocument( PersistableObject root );
		public abstract void Close();
	}

	public class XmlPersistanceReader : PersistanceReader
	{
		private readonly StreamReader m_Reader;
		private readonly XmlTextReader m_Xml;
		private readonly string m_Title;

		public XmlPersistanceReader( string filePath, string title )
		{
			m_Reader = new StreamReader( filePath );
			m_Xml = new XmlTextReader( m_Reader );
			m_Xml.WhitespaceHandling=WhitespaceHandling.None;
			m_Title = title;
		}

		public override int GetInt32( string key )
		{
			return XmlConvert.ToInt32( m_Xml.GetAttribute( key ) );
		}

		public override bool GetBoolean( string key )
		{
			return XmlConvert.ToBoolean( m_Xml.GetAttribute( key ) );
		}

		public override string GetString( string key )
		{
			return m_Xml.GetAttribute( key );
		}

		public override DateTime GetDateTime( string key )
		{
			string val = m_Xml.GetAttribute( key );

			if ( val == null )
				return DateTime.MinValue;

			return XmlConvert.ToDateTime( val, XmlDateTimeSerializationMode.Local );
		}

		private bool m_HasChild;

		public override bool HasChild => m_HasChild;

        private bool m_WasEmptyElement;

		public override bool BeginChildren()
		{
			m_HasChild = !m_WasEmptyElement;

			m_Xml.Read();

			return m_HasChild;
		}

		public override void FinishChildren()
		{
			m_Xml.Read();
		}

		public override PersistableObject GetChild()
		{
			PersistableType type = PersistableTypeRegistry.Find( m_Xml.Name );
			PersistableObject obj = type.Constructor();

			m_WasEmptyElement = m_Xml.IsEmptyElement;

			obj.Deserialize( this );

			m_HasChild = ( m_Xml.NodeType == XmlNodeType.Element );

			return obj;
		}

		public override void ReadDocument( PersistableObject root )
		{
            ConsoleLog.Write.Information($"Reports: {m_Title}: Loading..." );
			m_Xml.Read();
			m_Xml.Read();
			m_HasChild = !m_Xml.IsEmptyElement;
			root.Deserialize( this );
            ConsoleLog.Write.Information("done" );
		}

		public override void Close()
		{
			m_Xml.Close();
			m_Reader.Close();
		}
	}
}