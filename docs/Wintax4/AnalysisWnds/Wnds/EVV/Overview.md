### Overview

**The Edit VCH Value window** allows you to edit the value of specific Virtual Channels (VCH) directly from the layout, without opening the full Virtual Channel configuration. This function is intended for VCHs containing only a single numeric value and was introduced to simplify quick changes of constants directly within the layout. 

### Virtual Channel Requirements

• Only VCHs with the "Single Value" flag enabled can be added and edited in this window.

• VCHs with this flag must contain only one numeric value (e.g., VCH = 10).

• If a VCH marked as "Single Value" contains a formula or a reference to another channel, a validation error is raised during saving.