<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
    xmlns:s="http://library.by/catalog"
>
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/s:catalog/s:book">
    <xsl:element name="item">
      <xsl:element name="title">
        <xsl:value-of select="./s:title"/>
      </xsl:element>
      <xsl:element name="author">
        <xsl:value-of select="./s:author"/>
      </xsl:element>
      <xsl:element name="category">
        <xsl:value-of select="./s:genre"/>
      </xsl:element>
      <xsl:element name="link">
        <xsl:if test="./s:isbn and ./s:genre = 'Computer'">http://my.safaribooksonline.com/<xsl:value-of select="./s:isbn"/>/</xsl:if>
      </xsl:element>
      <xsl:element name="description">
        <xsl:value-of select="./s:description"/>
      </xsl:element>
      <xsl:element name="pubDate">
        <xsl:value-of select="./s:registration_date"/>
      </xsl:element>
      <xsl:element name="guid">
        <xsl:value-of select="@id"/>
      </xsl:element>
    </xsl:element>
  </xsl:template>
  
  <xsl:template match="/">
    <xsl:element name="rss">
      <xsl:attribute name="version">2.0</xsl:attribute>
      <xsl:element name="channel">
        <xsl:apply-templates select="/s:catalog/s:book"/>
      </xsl:element>
    </xsl:element>
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
