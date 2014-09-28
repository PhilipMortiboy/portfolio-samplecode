<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<h1>Search Results</h1>
		<xsl:for-each select="//twitch">
			<table>
				<tr>
					<td>
						<strong>Species: </strong><xsl:apply-templates select="species"/><br/>
						<strong>Date: </strong><xsl:apply-templates select="date"/><br/>
						<strong>Location: </strong>
					</td>
				</tr>
			</table>
			<p>
				<xsl:variable name="id" select="species"/>
				<a href="viewTwitch.php?id={id}"><img class="icon" src="images/view.png" alt="view" /></a>
			</p>
		</xsl:for-each>
   </xsl:template>
</xsl:stylesheet>