<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
    xmlns:s="http://library.by/catalog"
>
  <xsl:output method="html" version="4.0" encoding="UTF-8" indent="yes"/>
  <xsl:key name="groups" match="/s:catalog/s:book" use="./s:genre"/>

  <xsl:template match="/s:catalog">
    <xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
    <xsl:element name="html">
      <xsl:element name="head">
        <xsl:element name="title">Catalog Report</xsl:element>
      </xsl:element>
      <xsl:element name="body">
        <xsl:element name="h1">
          Report for <xsl:value-of select="s:GetDate()"/>
        </xsl:element>
        <xsl:apply-templates select="./s:book[generate-id() = generate-id(key('groups', ./s:genre)[1])]"/>
        <h2>
          Total books: <xsl:value-of select="count(/s:catalog/s:book)"/>
        </h2>
        
      </xsl:element>
    </xsl:element>
  </xsl:template>

  <xsl:template match="/s:catalog/s:book">
    <h1>
      <xsl:value-of select="./s:genre"/>
    </h1>
    <table>
      <tr class="heading">
        <th scope="col">Author</th>
        <th scope="col">Title</th>
        <th scope="col">Publish Date</th>
        <th scope="col">Registration Date</th>
      </tr>
      <xsl:for-each select="key('groups', ./s:genre)">
        <tr>
          <td>
            <xsl:value-of select="./s:author"/>
          </td>
          <td>
            <xsl:value-of select="./s:title"/>
          </td>
          <td>
            <xsl:value-of select="./s:publish_date"/>
          </td>
          <td>
            <xsl:value-of select="./s:registration_date"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
    <h3>
      Total: <xsl:value-of select="count(key('groups', ./s:genre))"/>
    </h3>
  </xsl:template>

  <msxsl:script language="CSharp" implements-prefix="s">
    public string GetDate()
    {
    return DateTime.Now.ToString("yyyy-MM-dd");
    }
  </msxsl:script>
  
</xsl:stylesheet>