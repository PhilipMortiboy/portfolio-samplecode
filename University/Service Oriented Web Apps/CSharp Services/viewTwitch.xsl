<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
   <xsl:template match="/">
		<h1>View Twitch</h1>
		<h2>Details</h2>
		<xsl:for-each select="//twitch">
			<p>
				<strong>Species: </strong><xsl:apply-templates select="species"/><br/>
				<strong>Age: </strong><xsl:apply-templates select="age"/><br/>
				<strong>Gender: </strong><xsl:apply-templates select="sex"/><br/>
				<strong>Date: </strong><xsl:apply-templates select="date"/><br/>
				<strong>Description: </strong>
			</p>
			<h2>Posted From</h2>
			<p>
				<strong>Location: </strong>
			</p>
		</xsl:for-each>
   </xsl:template>
</xsl:stylesheet>