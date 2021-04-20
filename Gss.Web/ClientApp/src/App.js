import React, { Component } from 'react';
import { AdminDashboard } from './components/Administration/Dashboard';


export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <AdminDashboard />
    );
  }
}
