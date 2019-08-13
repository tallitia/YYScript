using System;

public delegate void VoidEventHandler();

public delegate void EventHandler(object sender);

public delegate void DataChangeHandler(object data);

public delegate void CloseEventHandler(uint detail);

public delegate void ListEventHandler(string type, object data);