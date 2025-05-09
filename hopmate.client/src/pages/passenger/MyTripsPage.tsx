// client/src/pages/passenger/MyTripsPage.tsx

import React, { useState, useEffect } from 'react';
import { usePassengerTrips } from '../../hooks/usePassengerTrips';
import MyRequestCard from '../../components/passenger/MyRequestCard';
import { PassengerTripDto } from '../../types';
import Layout from '../../components/Layout';

const MyTripsPage: React.FC = () => {
    const { myRequests, isLoading, error, refreshData } = usePassengerTrips();
    const [activeTab, setActiveTab] = useState<string>('all');
    const [filteredRequests, setFilteredRequests] = useState<PassengerTripDto[]>([]);

    useEffect(() => {
        // Filter requests based on selected tab
        let filtered = [...myRequests];

        if (activeTab !== 'all') {
            filtered = filtered.filter(request =>
                request.requestStatus?.toLowerCase() === activeTab.toLowerCase()
            );
        }

        setFilteredRequests(filtered);
    }, [myRequests, activeTab]);

    const tabClasses = (tabName: string) =>
        `px-4 py-2 cursor-pointer ${activeTab === tabName
            ? 'border-b-2 border-blue-500 font-medium text-blue-600'
            : 'text-gray-600 hover:text-blue-500'}`;

    return (
        <Layout>
            <div className="container mx-auto px-4 py-8">
                <h1 className="text-2xl font-bold mb-6">My Trips</h1>

                <div className="bg-white shadow rounded-lg p-4 mb-6">
                    <div className="flex overflow-x-auto mb-4 border-b">
                        {['all', 'pending', 'accepted', 'rejected', 'waitinglist'].map((tab) => (
                            <div
                                key={tab}
                                className={tabClasses(tab)}
                                onClick={() => setActiveTab(tab)}
                            >
                                {tab === 'all' ? 'All' : tab.charAt(0).toUpperCase() + tab.slice(1)}
                                {tab !== 'all' && (
                                    <span className="ml-1 bg-gray-200 text-gray-800 text-xs font-medium px-2 py-0.5 rounded-full">
                                        {myRequests.filter(r => r.requestStatus?.toLowerCase() === tab).length}
                                    </span>
                                )}
                            </div>
                        ))}
                    </div>

                    {isLoading ? (
                        <div className="flex justify-center my-12">
                            Loading
                        </div>
                    ) : error ? (
                        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative mb-6">
                            {error}
                        </div>
                    ) : filteredRequests.length === 0 ? (
                        <div className="text-center py-8 text-gray-500">
                            No trip requests found
                        </div>
                    ) : (
                        <div className="space-y-4">
                            {filteredRequests.map((request) => (
                                <MyRequestCard
                                    key={request.id}
                                    request={request}
                                />
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </Layout>
    );
};

export default MyTripsPage;